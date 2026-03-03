import { ApiClient } from './client';

const PROJECTS_API_BASE_URL = import.meta.env.VITE_PROJECTS_API_BASE_URL || 'http://localhost:5144';

/**
 * ProjectSubtaskClient - Handles project subtask CRUD operations
 */
export class ProjectSubtaskClient extends ApiClient {
  constructor(baseURL = PROJECTS_API_BASE_URL) {
    super(baseURL);
  }

  /**
   * Get all project subtasks with pagination
   * GET /project-subtask?page=1&perPage=10
   * @param {number} page - Page number
   * @param {number} perPage - Items per page
   * @returns {Promise<Pagination<List<GetAllProjectSubtaskResponse>>>}
   */
  async getAll(page = 1, perPage = 10) {
    const response = await this.getClient().get('/project-subtask', {
      params: { page, perPage }
    });

    return response.data;
  }

  /**
   * Get project subtask by ID
   * GET /project-subtask/:id
   * @param {string} id - Project subtask ID
   * @returns {Promise<GetProjectSubtaskByIdResponse>}
   */
  async getById(id) {
    const response = await this.getClient().get(`/project-subtask/${id}`);
    return response.data;
  }

  /**
   * Create new project subtask
   * POST /project-subtask
   * @param {Object} projectSubtask - Project subtask data
   * @returns {Promise<AddProjectSubtaskResponse>}
   */
  async create(projectSubtask) {
    const response = await this.getClient().post('/project-subtask', projectSubtask);
    return response.data;
  }

  /**
   * Update project subtask
   * PATCH /project-subtask/:id
   * @param {string} id - Project subtask ID
   * @param {Object} projectSubtask - Project subtask data
   * @returns {Promise<UpdateProjectSubtaskResponse>}
   */
  async update(id, projectSubtask) {
    const response = await this.getClient().patch(`/project-subtask/${id}`, projectSubtask);
    return response.data;
  }

  /**
   * Delete project subtask
   * DELETE /project-subtask/:id
   * @param {string} id - Project subtask ID
   * @returns {Promise<void>}
   */
  async delete(id) {
    await this.getClient().delete(`/project-subtask/${id}`);
  }
}

export default ProjectSubtaskClient;
