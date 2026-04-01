import { AxiosResponse } from 'axios';
import type { Pagination } from '../../types';
import type {
  AddPositionCommand,
  AddPositionResponse,
  GetAllEmployeeResponse,
  GetAllPositionResponse,
  GetPositionByIdResponse,
  UpdatePositionCommand,
  UpdatePositionResponse
} from '../../types/basic-types';
import { ApiClient } from '../api-client';
import { BASIC_API_BASE_URL } from './basic-product-client';



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
   */
  async getAll(page: number = 1, perPage: number = 10): Promise<Pagination<GetAllPositionResponse>> {
    const response = await this.getClient().get('/position', {
      params: { page, perPage }
    });

    return (response as AxiosResponse).data;
  }

  /**
   * Get position by ID
   * GET /position/:id
   */
  async getById(id: string): Promise<GetPositionByIdResponse> {
    const response = await this.getClient().get(`/position/${id}`);
    return (response as AxiosResponse).data;
  }


  /**
   * Create new position
   * POST /position
   */
  async create(position: AddPositionCommand): Promise<AddPositionResponse> {
    const response = await this.getClient().post('/position', position);
    return (response as AxiosResponse).data;
  }


  /**
   * Update position
   * PATCH /position/:id
   */
  async update(id: string, position: UpdatePositionCommand): Promise<UpdatePositionResponse> {
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
   */
  async getEmployeesByPosition(id: string, page: number = 1, perPage: number = 10): Promise<Pagination<GetAllEmployeeResponse>> {
    const response = await this.getClient().get(`/position/${id}/employee`, {
      params: { page, perPage }
    });

    return (response as AxiosResponse).data;
  }

}

export default BasicPositionClient;
