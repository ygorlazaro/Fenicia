import { ApiClient } from './client';
import { AxiosResponse } from 'axios';

const BASIC_API_BASE_URL = import.meta.env.VITE_BASIC_API_BASE_URL || 'http://localhost:5083/api';

/**
 * BasicEmployeeClient - Handles employee CRUD operations
 */
export class BasicEmployeeClient extends ApiClient {
  constructor(baseURL: string = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  /**
   * Get all employees with pagination
   * GET /employee?page=1&perPage=10
   * @param {number} page - Page number
   * @param {number} perPage - Items per page
   * @returns {Promise<any>}
   */
  async getAll(page: number = 1, perPage: number = 10): Promise<any> {
    const response = await this.getClient().get('/employee', {
      params: { page, perPage }
    });

    return (response as AxiosResponse).data;
  }

  /**
   * Get employee by ID
   * GET /employee/:id
   * @param {string} id - Employee ID
   * @returns {Promise<any>}
   */
  async getById(id: string): Promise<any> {
    const response = await this.getClient().get(`/employee/${id}`);
    return (response as AxiosResponse).data;
  }

  /**
   * Create new employee
   * POST /employee
   * @param {any} employee - Employee data
   * @returns {Promise<any>}
   */
  async create(employee: any): Promise<any> {
    const response = await this.getClient().post('/employee', employee);
    return (response as AxiosResponse).data;
  }

  /**
   * Update employee
   * PATCH /employee/:id
   * @param {string} id - Employee ID
   * @param {any} employee - Employee data
   * @returns {Promise<any>}
   */
  async update(id: string, employee: any): Promise<any> {
    const response = await this.getClient().patch(`/employee/${id}`, employee);
    return (response as AxiosResponse).data;
  }

  /**
   * Delete employee
   * DELETE /employee/:id
   * @param {string} id - Employee ID
   * @returns {Promise<void>}
   */
  async delete(id: string): Promise<void> {
    await this.getClient().delete(`/employee/${id}`);
  }

  /**
   * Get all states
   * GET /state
   * @returns {Promise<any>}
   */
  async getStates(): Promise<any> {
    const response = await this.getClient().get('/state');
    return (response as AxiosResponse).data;
  }

  /**
   * Get all positions for data source
   * GET /datasource/position
   * @returns {Promise<any>}
   */
  async getPositions(): Promise<any> {
    const response = await this.getClient().get('/datasource/position');
    return (response as AxiosResponse).data;
  }
}

export default BasicEmployeeClient;
