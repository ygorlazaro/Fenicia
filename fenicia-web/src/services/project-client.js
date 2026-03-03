import { ApiClient } from './client';

const PROJECTS_API_BASE_URL = import.meta.env.VITE_PROJECTS_API_BASE_URL || 'http://localhost:5004/api';

/**
 * ProjectClient - Handles project CRUD operations
 */
export class ProjectClient extends ApiClient {
  constructor(baseURL = PROJECTS_API_BASE_URL) {
    super(baseURL);
  }

  /**
   * Get all projects with pagination
   * GET /project?page=1&perPage=10
   * @param {number} page - Page number
   * @param {number} perPage - Items per page
   * @returns {Promise<Pagination<List<GetAllProjectResponse>>>}
   */
  async getAll(page = 1, perPage = 10) {
    const response = await this.getClient().get('/project', {
      params: { page, perPage }
    });

    return response.data;
  }

  /**
   * Get project by ID
   * GET /project/:id
   * @param {string} id - Project ID
   * @returns {Promise<GetProjectByIdResponse>}
   */
  async getById(id) {
    const response = await this.getClient().get(`/project/${id}`);
    return response.data;
  }

  /**
   * Create new project
   * POST /project
   * @param {Object} project - Project data
   * @returns {Promise<AddProjectResponse>}
   */
  async create(project) {
    const response = await this.getClient().post('/project', project);
    return response.data;
  }

  /**
   * Update project
   * PATCH /project/:id
   * @param {string} id - Project ID
   * @param {Object} project - Project data
   * @returns {Promise<UpdateProjectResponse>}
   */
  async update(id, project) {
    const response = await this.getClient().patch(`/project/${id}`, project);
    return response.data;
  }

  /**
   * Delete project
   * DELETE /project/:id
   * @param {string} id - Project ID
   * @returns {Promise<void>}
   */
  async delete(id) {
    await this.getClient().delete(`/project/${id}`);
  }
}

export default ProjectClient;
