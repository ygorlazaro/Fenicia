import {ApiClient} from "src/services/client";

const BASIC_API_BASE_URL = import.meta.env.VITE_BASIC_API_BASE_URL || 'http://localhost:5002/api';

/**
 * BasicEmployeeClient - Handles employee CRUD operations
 */
export class BasicEmployeeClient extends ApiClient {
  constructor(baseURL = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  /**
   * Get all employees with pagination
   * GET /employee?page=1&perPage=10
   * @param {number} page - Page number
   * @param {number} perPage - Items per page
   * @returns {Promise<Pagination<List<GetAllEmployeeResponse>>>}
   */
  async getAll(page = 1, perPage = 10) {
    const response = await this.getClient().get('/employee', {
      params: { page, perPage }
    });

    return response.data;
  }

  /**
   * Get employee by ID
   * GET /employee/:id
   * @param {string} id - Employee ID
   * @returns {Promise<GetEmployeeByIdResponse>}
   */
  async getById(id) {
    const response = await this.getClient().get(`/employee/${id}`);
    return response.data;
  }

  /**
   * Create new employee
   * POST /employee
   * @param {Object} employee - Employee data
   * @returns {Promise<AddEmployeeResponse>}
   */
  async create(employee) {
    const response = await this.getClient().post('/employee', employee);
    return response.data;
  }

  /**
   * Update employee
   * PATCH /employee/:id
   * @param {string} id - Employee ID
   * @param {Object} employee - Employee data
   * @returns {Promise<UpdateEmployeeResponse>}
   */
  async update(id, employee) {
    const response = await this.getClient().patch(`/employee/${id}`, employee);
    return response.data;
  }

  /**
   * Delete employee
   * DELETE /employee/:id
   * @param {string} id - Employee ID
   * @returns {Promise<void>}
   */
  async delete(id) {
    await this.getClient().delete(`/employee/${id}`);
  }

  /**
   * Get all states
   * GET /state
   * @returns {Promise<List<GetAllStateResponse>>}
   */
  async getStates() {
    const response = await this.getClient().get('/state');
    return response.data;
  }

  /**
   * Get all positions
   * GET /position
   * @returns {Promise<List<GetAllPositionResponse>>}
   */
  async getPositions() {
    const response = await this.getClient().get('/position');
    return response.data;
  }
}

export default BasicEmployeeClient;
