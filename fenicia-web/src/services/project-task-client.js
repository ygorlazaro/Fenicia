import { ApiClient } from './client';

const PROJECTS_API_BASE_URL = import.meta.env.VITE_PROJECTS_API_BASE_URL || 'http://localhost:5144';

/**
 * ProjectTaskClient - Handles project task CRUD operations
 */
export class ProjectTaskClient extends ApiClient {
  constructor(baseURL = PROJECTS_API_BASE_URL) {
    super(baseURL);
  }

  /**
   * Get all project tasks with pagination
   * GET /project-task?page=1&perPage=10
   * @param {number} page - Page number
   * @param {number} perPage - Items per page
   * @returns {Promise<Pagination<List<GetAllProjectTaskResponse>>>}
   */
  async getAll(page = 1, perPage = 10) {
    const response = await this.getClient().get('/project-task', {
      params: { page, perPage }
    });

    return response.data;
  }

  /**
   * Get project task by ID
   * GET /project-task/:id
   * @param {string} id - Project task ID
   * @returns {Promise<GetProjectTaskByIdResponse>}
   */
  async getById(id) {
    const response = await this.getClient().get(`/project-task/${id}`);
    return response.data;
  }

  /**
   * Create new project task
   * POST /project-task
   * @param {Object} projectTask - Project task data
   * @returns {Promise<AddProjectTaskResponse>}
   */
  async create(projectTask) {
    const response = await this.getClient().post('/project-task', projectTask);
    return response.data;
  }

  /**
   * Update project task
   * PATCH /project-task/:id
   * @param {string} id - Project task ID
   * @param {Object} projectTask - Project task data
   * @returns {Promise<UpdateProjectTaskResponse>}
   */
  async update(id, projectTask) {
    const response = await this.getClient().patch(`/project-task/${id}`, projectTask);
    return response.data;
  }

  /**
   * Delete project task
   * DELETE /project-task/:id
   * @param {string} id - Project task ID
   * @returns {Promise<void>}
   */
  async delete(id) {
    await this.getClient().delete(`/project-task/${id}`);
  }
}

export default ProjectTaskClient;
