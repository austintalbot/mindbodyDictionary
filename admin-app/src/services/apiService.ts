// admin-app/src/services/apiService.ts

import {
  ADMIN_API_URL,
  DELETE_MBD_CONDITION_CODE,
  UPSERT_MBD_CONDITION_CODE,
  DELETE_IMAGE_CODE,
  CONTACTS_TABLE_CODE,
  DELETE_CONTACT_CODE,
  SEND_PUSH_NOTIFICATION_CODE,
  CREATE_BACKUP_CODE,
  RESTORE_DATABASE_CODE,
  GET_MBD_CONDITIONS_CODE,
  GET_MBD_IMAGES_CODE,
  MBD_CONDITION_CODE,
  FAQ_FUNCTION_CODE,
  UPDATE_FAQS_ORDER_CODE,
  COSMOS_DB_CONTAINER_NAME,
} from '../constants';
import { MbdCondition, Faq, MbdMovementLink } from '../types'; // Import from barrel file

// In-memory caches
let mbdConditionsCache: any[] | null = null;
let imagesCache: any[] | null = null;

// Helper for making API requests
async function makeApiRequest<T>(
  endpoint: string,
  method: string = 'GET',
  data?: any,
  contentType: string = 'application/json'
): Promise<T> {
  const url = `${ADMIN_API_URL}/api/${endpoint}`;
  const options: RequestInit = {
    method,
    headers: {
      // 'Content-Type': contentType, // Set dynamically below if data exists
    },
  };

  if (data) {
    if (data instanceof FormData) {
      // When sending FormData, browsers set Content-Type automatically,
      // and we should NOT set it manually, or boundaries will be missing.
      options.body = data;
    } else {
      options.headers = {
        'Content-Type': contentType,
      } as HeadersInit;
      options.body = JSON.stringify(data);
    }
  }

  const response = await fetch(url, options);

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(`API request failed: ${response.status} ${response.statusText} - ${errorText}`);
  }

  // Check if the response has content before parsing as JSON
  const responseText = await response.text();
  console.log('API Raw Response Text:', endpoint, responseText); // Added log
  const responseData = responseText ? JSON.parse(responseText) : ({} as T);

  if (method === 'GET') {
    let count = 0;
    if (Array.isArray(responseData)) {
      count = responseData.length;
    } else if (responseData && (responseData as any).data && Array.isArray((responseData as any).data)) {
      count = (responseData as any).data.length;
    } else if (responseData && Object.keys(responseData).length > 0) {
      count = 1; // Assume a single object if not an array and not empty
    }
    console.log(`[API] GET ${endpoint} returned ${count} items.`);
  }

  return responseData;
}

// --- Cache Management Functions ---
export const clearMbdConditionsCache = () => {
  mbdConditionsCache = null;
  console.log('MBD Conditions Cache cleared.');
};

export const clearImagesCache = () => {
  imagesCache = null;
  console.log('Images Cache cleared.');
};


// --- MbdCondition API Calls ---
export const fetchMbdConditions = async (): Promise<any> => {
  if (mbdConditionsCache) {
    console.log('Returning MBD Conditions from cache.');
    return mbdConditionsCache;
  }
  const response = await makeApiRequest<any>(`GetMbdConditions?code=${GET_MBD_CONDITIONS_CODE}`);
  const data = (response as any).data || response; // Handle { data: [...] } or direct array
  mbdConditionsCache = data;
  return data;
};

export const fetchMbdCondition = async (requestedId: string, name: string): Promise<MbdCondition> => {
  console.log("fetchMbdCondition called with - requestedId:", requestedId, "name:", name);
  const decodedName = name.replace("paranthesis", "'");

  // Attempt 1: Fetch all conditions and perform client-side filtering.
  // This is for scenarios where GetMbdConditions (plural) is used as primary data source,
  // but doesn't filter server-side effectively by ID/Name.
  try {
    const responseAll = await makeApiRequest<MbdCondition[]>(`GetMbdConditions?code=${GET_MBD_CONDITIONS_CODE}&id=${requestedId}&name=${encodeURIComponent(decodedName)}`);
    console.log("API response from GetMbdConditions (plural):", responseAll.slice(0, 5)); // Log first few items
    console.log("All IDs in GetMbdConditions response:", responseAll.map(a => a.id));

    if (responseAll && Array.isArray(responseAll)) {
      const foundAilmentById = responseAll.find(apiAilment => {
        console.log(`(Attempt 1 - ID) Comparing requested ID "${requestedId}" with API ailment ID "${apiAilment.id}"`);
        return apiAilment.id === requestedId;
      });
      if (foundAilmentById) {
        console.log("Client-side foundAilment (by ID) from GetMbdConditions:", foundAilmentById);
        return foundAilmentById;
      }

      const foundAilmentByName = responseAll.find(apiAilment => {
        console.log(`(Attempt 1 - Name) Comparing requested NAME "${name}" with API ailment NAME "${apiAilment.name}"`);
        return apiAilment.name === name;
      });
      if (foundAilmentByName) {
        console.log("Client-side foundAilment (by Name) from GetMbdConditions:", foundAilmentByName);
        return foundAilmentByName;
      }
    }
  } catch (error) {
    console.warn("Client-side filtering from GetMbdConditions failed:", error);
  }

  // Attempt 2: If client-side filtering fails, leverage the singular GetMbdCondition endpoint.
  // This assumes MBD_CONDITION_CODE corresponds to a specific GetMbdCondition endpoint.
  try {
    console.log("Attempt 2: Falling back to singular GetMbdCondition endpoint for ID:", requestedId);
    // Note: The `name` parameter is typically not needed for a singular endpoint by ID.
    const singularMbdCondition = await makeApiRequest<MbdCondition>(`GetMbdCondition?code=${MBD_CONDITION_CODE}&id=${requestedId}`);
    console.log("Response from singular GetMbdCondition:", singularMbdCondition);
    return singularMbdCondition;
  } catch (error) {
    console.warn("Fallback to singular GetMbdCondition endpoint failed:", error);
  }

  // If both attempts fail, return a default empty MbdCondition object.
  return {
    id: '', name: '', subscriptionOnly: false, summaryNegative: '', summaryPositive: '',
    affirmations: [], physicalConnections: [], searchTags: [], tags: [], recommendations: []
  };
};

export const upsertMbdCondition = async (mbdCondition: MbdCondition): Promise<MbdCondition> => {
  clearMbdConditionsCache(); // Clear cache on data modification
  return makeApiRequest<MbdCondition>(`UpsertAilment?code=${UPSERT_MBD_CONDITION_CODE}`, 'POST', mbdCondition);
};

export const deleteMbdCondition = async (id: string, name: string): Promise<void> => {
  clearMbdConditionsCache(); // Clear cache on data modification
  const decodedName = name.replace("paranthesis", "'");
  return makeApiRequest<void>(`DeleteAilment?code=${DELETE_MBD_CONDITION_CODE}&id=${id}&name=${encodeURIComponent(decodedName)}`, 'POST');
};

// --- Image API Calls ---
export const fetchImagesTable = async (): Promise<any[]> => {
  if (imagesCache) {
    console.log('Returning Images from cache.');
    return imagesCache;
  }
  const response = await makeApiRequest<any[]>(`GetMbdImages?code=${GET_MBD_IMAGES_CODE}`);
  const data = (response as any).data || response; // Handle { data: [...] } or direct array
  
  const mappedData = Array.isArray(data) ? data.map((item: any) => ({
      ...item,
      mbdCondition: item.ailment || item.Ailment || item.mbdCondition
  })) : [];

  imagesCache = mappedData;
  return mappedData;
};

export const deleteImage = async (imageName: string): Promise<void> => {
  clearImagesCache(); // Clear cache on data modification
  const url = `deletembdimage?code=${DELETE_IMAGE_CODE}&name=${encodeURIComponent(imageName)}`;
  return makeApiRequest<void>(url, 'POST');
};

export const uploadImage = async (ailmentName: string, imageType: string, file: File): Promise<any> => {
  clearImagesCache(); // Clear cache on data modification
  const name = `${ailmentName}${imageType}.png`;
  const formData = new FormData();
  formData.append('file', file);
  return makeApiRequest<any>(`Image?name=${encodeURIComponent(name)}`, 'POST', formData);
};

// --- Contact API Calls ---
export const fetchContactsTable = async (): Promise<any[]> => {
  return makeApiRequest<any[]>(`ContactsTables?code=${CONTACTS_TABLE_CODE}`);
};

export const deleteContact = async (id: string, email: string): Promise<void> => {
  return makeApiRequest<void>(`DeleteContact?code=${DELETE_CONTACT_CODE}&id=${id}&name=${encodeURIComponent(email)}`, 'POST');
};

// --- Notification API Calls ---
export interface NotificationPayload {
  Title: string;
  Body: string;
  SubscribersOnly: string;
  AilmentId: string;
}

export const sendPushNotification = async (notification: NotificationPayload): Promise<void> => {
  return makeApiRequest<void>(`SendPushNotification?code=${SEND_PUSH_NOTIFICATION_CODE}`, 'POST', notification);
};

// --- Database API Calls ---
export const createBackupUrl = (databaseName: string = COSMOS_DB_CONTAINER_NAME): string => {
    return `${ADMIN_API_URL}/api/CreateBackup?code=${CREATE_BACKUP_CODE}&databaseName=${encodeURIComponent(databaseName)}`;
};

export const restoreDatabase = async (file: File): Promise<void> => {
  const formData = new FormData();
  formData.append('File', file);
  return makeApiRequest<void>(`RestoreDatabase?code=${RESTORE_DATABASE_CODE}`, 'POST', formData);
};

// --- FAQ API Calls ---
export const fetchFaqs = async (): Promise<Faq[]> => {
  const response = await makeApiRequest<any>(`GetFaqs?code=${FAQ_FUNCTION_CODE}`);
  return (response as any).data || response;
};

export const upsertFaq = async (faq: Faq): Promise<Faq> => {
  return makeApiRequest<Faq>(`UpsertFaq?code=${FAQ_FUNCTION_CODE}`, 'POST', faq);
};

export const deleteFaq = async (id: string): Promise<void> => {
  return makeApiRequest<void>(`DeleteFaq?code=${FAQ_FUNCTION_CODE}&id=${id}`, 'POST');
};

export const updateFaqsOrder = async (faqs: Faq[]): Promise<void> => {
  return makeApiRequest<void>(`UpdateFaqsOrder?code=${UPDATE_FAQS_ORDER_CODE}`, 'POST', faqs);
};

// --- Movement Link API Calls ---
export const fetchMovementLinks = async (): Promise<MbdMovementLink[]> => {
  const response = await makeApiRequest<any>(`GetMbdMovementLinks`);
  return (response as any).data || response;
};

export const upsertMovementLink = async (link: MbdMovementLink): Promise<MbdMovementLink> => {
  return makeApiRequest<MbdMovementLink>(`UpsertMbdMovementLink`, 'POST', link);
};

export const deleteMovementLink = async (id: string): Promise<void> => {
  return makeApiRequest<void>(`DeleteMbdMovementLink?id=${id}`, 'POST');
};

export const updateMovementLinksOrder = async (links: MbdMovementLink[]): Promise<void> => {
  return makeApiRequest<void>(`UpdateMbdMovementLinksOrder`, 'POST', links);
};
