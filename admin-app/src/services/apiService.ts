// admin-app/src/services/apiService.ts

import {
  ADMIN_API_URL,
  DELETE_AILMENT_CODE,
  UPSERT_AILMENT_CODE,
  IMAGES_TABLE_CODE,
  DELETE_IMAGE_CODE,
  CONTACTS_TABLE_CODE,
  DELETE_CONTACT_CODE,
  SEND_PUSH_NOTIFICATION_CODE,
  CREATE_BACKUP_CODE,
  RESTORE_DATABASE_CODE,
  GET_MBD_CONDITIONS_TABLE_CODE,
  GET_MBD_CONDITIONS_CODE,
} from '../constants';
import { MbdCondition } from '../types'; // Import from barrel file

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
  return responseText ? JSON.parse(responseText) : ({} as T);
}

// --- MbdCondition API Calls ---
export const fetchMbdConditionsTable = async (): Promise<any> => { // The API returns {"data":[]} so it returns 'any' for now.
  return makeApiRequest<any>(`GetMbdConditionsTable?code=${GET_MBD_CONDITIONS_TABLE_CODE}`);
};

export const fetchMbdCondition = async (id: string, name: string): Promise<MbdCondition> => {
  // Original JS replaced single quote with 'paranthesis' - we need to reverse that for the actual API call
  const decodedName = name.replace("paranthesis", "'");
  return makeApiRequest<MbdCondition>(`GetMbdConditions?code=${GET_MBD_CONDITIONS_CODE}&id=${id}&name=${encodeURIComponent(decodedName)}`);
};

export const upsertAilment = async (ailment: MbdCondition): Promise<MbdCondition> => {
  return makeApiRequest<MbdCondition>(`UpsertAilment?code=${UPSERT_AILMENT_CODE}`, 'POST', ailment);
};

export const deleteAilment = async (id: string, name: string): Promise<void> => {
  const decodedName = name.replace("paranthesis", "'");
  return makeApiRequest<void>(`DeleteAilment?code=${DELETE_AILMENT_CODE}&id=${id}&name=${encodeURIComponent(decodedName)}`, 'POST');
};

// --- Image API Calls ---
export const fetchImagesTable = async (): Promise<any[]> => {
  return makeApiRequest<any[]>(`ImagesTable?code=${IMAGES_TABLE_CODE}`);
};

export const deleteImage = async (imageName: string): Promise<void> => {
  return makeApiRequest<void>(`DeleteImage?code=${DELETE_IMAGE_CODE}&name=${encodeURIComponent(imageName)}`, 'POST');
};

export const uploadImage = async (ailmentName: string, imageType: string, file: File): Promise<any> => {
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
export const createBackupUrl = (): string => {
    return `${ADMIN_API_URL}/api/CreateBackup?code=${CREATE_BACKUP_CODE}`;
};

export const restoreDatabase = async (file: File): Promise<void> => {
  const formData = new FormData();
  formData.append('File', file);
  return makeApiRequest<void>(`RestoreDatabase?code=${RESTORE_DATABASE_CODE}`, 'POST', formData);
};

// Expose image base URL for direct use in image src attributes
// export const getImageBaseUrl = (): string => IMAGE_BASE_URL; // Moved to constants.ts
