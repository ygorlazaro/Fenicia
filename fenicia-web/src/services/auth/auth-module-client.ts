import { AxiosResponse } from 'axios';
import { Pagination } from '../../types';
import { GetModuleResponse, UserModuleResponse, UserSubscriptionResponse } from '../../types/auth-types';
import { ApiClient } from '../api-client';

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
   * @returns {Promise<Pagination<GetModuleResponse>>}
   */
  async getModules(page: number = 1, perPage: number = 50): Promise<Pagination<GetModuleResponse>> {
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
    const moduleIds: string[] = profile.subscriptions
      .flatMap((subscription: UserSubscriptionResponse) =>
      subscription.modules.map((module: UserModuleResponse) => module.id)
    );

    return [...new Set(moduleIds)];
  }
}

export default AuthModuleClient;
