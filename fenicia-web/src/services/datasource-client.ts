import { ApiClient } from './client';
import { AxiosResponse } from 'axios';

const BASIC_API_BASE_URL = import.meta.env.VITE_BASIC_API_BASE_URL || 'http://localhost:5002/api';

export class DataSourceClient extends ApiClient {
  constructor(baseURL: string = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  async getPositions(): Promise<any> {
    const response = await this.getClient().get('/datasource/position');
    return (response as AxiosResponse).data;
  }
}

export default DataSourceClient;
