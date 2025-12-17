// admin-app/src/constants.ts

export const ADMIN_API_URL = import.meta.env.VITE_ADMIN_API_URL || 'http://localhost:7071'; // Default to a local Azure Function host
export const IMAGE_BASE_URL = import.meta.env.VITE_IMAGE_BASE_URL || 'https://mbdstoragesa.blob.core.windows.net/mbdconditionimages';

export const MBD_CONDITIONS_TABLE_CODE = import.meta.env.VITE_AILMENTS_TABLE_CODE || 'YOUR_AILMENTS_TABLE_CODE';
export const MBD_CONDITION_CODE = import.meta.env.VITE_AILMENT_CODE || 'YOUR_AILMENT_CODE';
export const DELETE_MBD_CONDITION_CODE = import.meta.env.VITE_DELETE_AILMENT_CODE || 'YOUR_DELETE_AILMENT_CODE';
export const UPSERT_MBD_CONDITION_CODE = import.meta.env.VITE_UPSERT_AILMENT_CODE || 'YOUR_UPSERT_AILMENT_CODE';
export const IMAGES_TABLE_CODE = import.meta.env.VITE_IMAGES_TABLE_CODE || 'YOUR_IMAGES_TABLE_CODE';
export const DELETE_IMAGE_CODE = import.meta.env.VITE_DELETE_IMAGE_CODE || 'YOUR_DELETE_IMAGE_CODE';
export const CONTACTS_TABLE_CODE = import.meta.env.VITE_CONTACTS_TABLE_CODE || 'YOUR_CONTACTS_TABLE_CODE';
export const DELETE_CONTACT_CODE = import.meta.env.VITE_DELETE_CONTACT_CODE || 'YOUR_DELETE_CONTACT_CODE';
export const SEND_PUSH_NOTIFICATION_CODE = import.meta.env.VITE_SEND_PUSH_NOTIFICATION_CODE || 'YOUR_SEND_PUSH_NOTIFICATION_CODE';
export const CREATE_BACKUP_CODE = import.meta.env.VITE_CREATE_BACKUP_CODE || 'YOUR_CREATE_BACKUP_CODE';
export const RESTORE_DATABASE_CODE = import.meta.env.VITE_RESTORE_DATABASE_CODE || 'YOUR_RESTORE_DATABASE_CODE';
export const GET_MBD_CONDITIONS_TABLE_CODE = import.meta.env.VITE_GET_MBD_CONDITIONS_TABLE_CODE || 'YOUR_GET_MBD_CONDITIONS_TABLE_CODE';
export const GET_MBD_CONDITIONS_CODE = import.meta.env.VITE_GET_MBD_CONDITIONS_CODE || 'YOUR_GET_MBD_CONDITIONS_CODE';
export const GET_MBD_IMAGES_CODE = import.meta.env.VITE_GET_MBD_IMAGES_CODE || 'YOUR_GET_MBD_IMAGES_CODE';
export const FAQ_FUNCTION_CODE = import.meta.env.VITE_FAQ_FUNCTION_CODE || 'p8_sBm-IGx0vcvseYZK_mGxL16_CYCbH7RgPb2p-YoIkAzFuiNtQ1Q==';
export const UPDATE_FAQS_ORDER_CODE = import.meta.env.VITE_UPDATE_FAQS_ORDER_CODE || 'p8_sBm-IGx0vcvseYZK_mGxL16_CYCbH7RgPb2p-YoIkAzFuiNtQ1Q==';
export const COSMOS_DB_CONTAINER_NAME = import.meta.env.VITE_COSMOS_DB_CONTAINER_NAME || 'mindbody';

// Expose image base URL for direct use in image src attributes
export const getImageBaseUrl = (): string => IMAGE_BASE_URL;
