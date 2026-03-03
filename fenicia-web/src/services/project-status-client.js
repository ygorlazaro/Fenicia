import { ApiClient } from './client';

const PROJECTS_API_BASE_URL = import.meta.env.VITE_PROJECTS_API_BASE_URL || 'http://localhost:5144';

/**
 * ProjectStatusClient - Handles project status CRUD operations
 */
export class ProjectStatusClient extends ApiClient {
  constructor(baseURL = PROJECTS_API_BASE_URL) {
    super(baseURL);
  }

  /**
   * Get all project statuses with pagination
   * GET /project-status?page=1&perPage=10
   * @param {number} page - Page number
   * @param {number} perPage - Items per page
   * @returns {Promise<Pagination<List<GetAllProjectStatusResponse>>>}
   */
  async getAll(page = 1, perPage = 10) {
    const response = await this.getClient().get('/project-status', {
      params: { page, perPage }
    });

    return response.data;
  }

  /**
   * Get project status by ID
   * GET /project-status/:id
   * @param {string} id - Project status ID
   * @returns {Promise<GetProjectStatusByIdResponse>}
   */
  async getById(id) {
    const response = await this.getClient().get(`/project-status/${id}`);
    return response.data;
  }

  /**
   * Create new project status
   * POST /project-status
   * @param {Object} projectStatus - Project status data
   * @returns {Promise<AddProjectStatusResponse>}
   */
  async create(projectStatus) {
    const response = await this.getClient().post('/project-status', projectStatus);
    return response.data;
  }

  /**
   * Update project status
   * PATCH /project-status/:id
   * @param {string} id - Project status ID
   * @param {Object} projectStatus - Project status data
   * @returns {Promise<UpdateProjectStatusResponse>}
   */
  async update(id, projectStatus) {
    const response = await this.getClient().patch(`/project-status/${id}`, projectStatus);
    return response.data;
  }

  /**
   * Delete project status
   * DELETE /project-status/:id
   * @param {string} id - Project status ID
   * @returns {Promise<void>}
   */
  async delete(id) {
    await this.getClient().delete(`/project-status/${id}`);
  }
}

export default ProjectStatusClient;
