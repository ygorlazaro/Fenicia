import {ApiClient} from "src/services/client";

const BASIC_API_BASE_URL = import.meta.env.VITE_BASIC_API_BASE_URL || 'http://localhost:5002/api';

/**
 * BasicPositionClient - Handles position CRUD operations
 */
export class BasicPositionClient extends ApiClient {
  constructor(baseURL = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  /**
   * Get all positions with pagination
   * GET /position?page=1&perPage=10
   * @param {number} page - Page number
   * @param {number} perPage - Items per page
   * @returns {Promise<Pagination<List<GetAllPositionResponse>>>}
   */
  async getAll(page = 1, perPage = 10) {
    const response = await this.getClient().get('/position', {
      params: { page, perPage }
    });

    return response.data;
  }

  /**
   * Get position by ID
   * GET /position/:id
   * @param {string} id - Position ID
   * @returns {Promise<GetPositionByIdResponse>}
   */
  async getById(id) {
    const response = await this.getClient().get(`/position/${id}`);
    return response.data;
  }

  /**
   * Create new position
   * POST /position
   * @param {Object} position - Position data
   * @returns {Promise<AddPositionResponse>}
   */
  async create(position) {
    const response = await this.getClient().post('/position', position);
    return response.data;
  }

  /**
   * Update position
   * PATCH /position/:id
   * @param {string} id - Position ID
   * @param {Object} position - Position data
   * @returns {Promise<UpdatePositionResponse>}
   */
  async update(id, position) {
    const response = await this.getClient().patch(`/position/${id}`, position);
    return response.data;
  }

  /**
   * Delete position
   * DELETE /position/:id
   * @param {string} id - Position ID
   * @returns {Promise<void>}
   */
  async delete(id) {
    await this.getClient().delete(`/position/${id}`);
  }

  /**
   * Get employees by position ID
   * GET /position/:id/employee
   * @param {string} id - Position ID
   * @param {number} page - Page number
   * @param {number} perPage - Items per page
   * @returns {Promise<List<GetEmployeesByPositionIdResponse>>}
   */
  async getEmployeesByPosition(id, page = 1, perPage = 10) {
    const response = await this.getClient().get(`/position/${id}/employee`, {
      params: { page, perPage }
    });

    return response.data;
  }
}

export default BasicPositionClient;
