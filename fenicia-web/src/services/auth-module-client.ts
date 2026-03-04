import { ApiClient } from './client';
import { AxiosResponse } from 'axios';

const AUTH_API_BASE_URL = import.meta.env.VITE_AUTH_API_BASE_URL || 'http://localhost:5001/api';

/**
 * AuthModuleClient - Handles module-related operations
 * Implements fetching available modules
 */
export class AuthModuleClient extends ApiClient {
  constructor(baseURL: string = AUTH_API_BASE_URL) {
    super(baseURL);
  }

  /**
   * Get all available modules
   * GET /module?page=1&perPage=10
   * @param {number} page - Page number
   * @param {number} perPage - Items per page
   * @returns {Promise<any>}
   */
  async getModules(page: number = 1, perPage: number = 50): Promise<any> {
    const response = await this.getClient().get('/module', {
      params: { page, perPage }
    });

    return (response as AxiosResponse).data;
  }

  /**
   * Get all subscribed module IDs for the current user
   * Fetches profile and extracts module IDs from all subscriptions
   * @returns {Promise<string[]>} Array of subscribed module IDs
   */
  async getSubscribedModuleIds(): Promise<string[]> {
    const response = await this.getClient().get('/subscription/profile');
    const profile = (response as AxiosResponse).data;

    if (!profile.subscriptions || profile.subscriptions.length === 0) {
      return [];
    }

    // Extract all module IDs from all subscriptions
    const moduleIds = profile.subscriptions.flatMap((subscription: any) =>
      subscription.modules.map((module: any) => module.id)
    );

    return moduleIds;
  }
}

export default AuthModuleClient;
