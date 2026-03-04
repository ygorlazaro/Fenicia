import { ApiClient } from './client';
import { AxiosResponse } from 'axios';

const BASIC_API_BASE_URL = import.meta.env.VITE_BASIC_API_BASE_URL || 'http://localhost:5002/api';

/**
 * BasicPositionClient - Handles position CRUD operations
 */
export class BasicPositionClient extends ApiClient {
  constructor(baseURL: string = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  /**
   * Get all positions with pagination
   * GET /position?page=1&perPage=10
   * @param {number} page - Page number
   * @param {number} perPage - Items per page
   * @returns {Promise<any>}
   */
  async getAll(page: number = 1, perPage: number = 10): Promise<any> {
    const response = await this.getClient().get('/position', {
      params: { page, perPage }
    });

    return (response as AxiosResponse).data;
  }

  /**
   * Get position by ID
   * GET /position/:id
   * @param {string} id - Position ID
   * @returns {Promise<any>}
   */
  async getById(id: string): Promise<any> {
    const response = await this.getClient().get(`/position/${id}`);
    return (response as AxiosResponse).data;
  }

  /**
   * Create new position
   * POST /position
   * @param {Object} position - Position data
   * @returns {Promise<any>}
   */
  async create(position: any): Promise<any> {
    const response = await this.getClient().post('/position', position);
    return (response as AxiosResponse).data;
  }

  /**
   * Update position
   * PATCH /position/:id
   * @param {string} id - Position ID
   * @param {Object} position - Position data
   * @returns {Promise<any>}
   */
  async update(id: string, position: any): Promise<any> {
    const response = await this.getClient().patch(`/position/${id}`, position);
    return (response as AxiosResponse).data;
  }

  /**
   * Delete position
   * DELETE /position/:id
   * @param {string} id - Position ID
   * @returns {Promise<void>}
   */
  async delete(id: string): Promise<void> {
    await this.getClient().delete(`/position/${id}`);
  }

  /**
   * Get employees by position ID
   * GET /position/:id/employee
   * @param {string} id - Position ID
   * @param {number} page - Page number
   * @param {number} perPage - Items per page
   * @returns {Promise<any>}
   */
  async getEmployeesByPosition(id: string, page: number = 1, perPage: number = 10): Promise<any> {
    const response = await this.getClient().get(`/position/${id}/employee`, {
      params: { page, perPage }
    });

    return (response as AxiosResponse).data;
  }
}

export default BasicPositionClient;
