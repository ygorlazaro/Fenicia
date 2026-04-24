import { AxiosResponse } from 'axios';
import { GetAllStateResponse } from "../../types/basic/state/get-all-state-response";
import { ApiClient } from '../api-client';
import { BASIC_API_BASE_URL } from './basic-product-client';

/**
 * Basic State Client - Handles state CRUD operations
 */

export class BasicStateClient extends ApiClient {
  constructor(baseURL: string = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  async getStates(): Promise<GetAllStateResponse[]> {
    const response = await this.getClient().get('/state');
    return (response as AxiosResponse).data;
  }
}
