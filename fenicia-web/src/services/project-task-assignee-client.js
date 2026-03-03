import { ApiClient } from './client';

const PROJECTS_API_BASE_URL = import.meta.env.VITE_PROJECTS_API_BASE_URL || 'http://localhost:5144';

/**
 * ProjectTaskAssigneeClient - Handles project task assignee CRUD operations
 */
export class ProjectTaskAssigneeClient extends ApiClient {
  constructor(baseURL = PROJECTS_API_BASE_URL) {
    super(baseURL);
  }

  /**
   * Get all project task assignees with pagination
   * GET /project-task-assignee?page=1&perPage=10
   * @param {number} page - Page number
   * @param {number} perPage - Items per page
   * @returns {Promise<Pagination<List<GetAllProjectTaskAssigneeResponse>>>}
   */
  async getAll(page = 1, perPage = 10) {
    const response = await this.getClient().get('/project-task-assignee', {
      params: { page, perPage }
    });

    return response.data;
  }

  /**
   * Get project task assignee by ID
   * GET /project-task-assignee/:id
   * @param {string} id - Project task assignee ID
   * @returns {Promise<GetProjectTaskAssigneeByIdResponse>}
   */
  async getById(id) {
    const response = await this.getClient().get(`/project-task-assignee/${id}`);
    return response.data;
  }

  /**
   * Create new project task assignee
   * POST /project-task-assignee
   * @param {Object} projectTaskAssignee - Project task assignee data
   * @returns {Promise<AddProjectTaskAssigneeResponse>}
   */
  async create(projectTaskAssignee) {
    const response = await this.getClient().post('/project-task-assignee', projectTaskAssignee);
    return response.data;
  }

  /**
   * Update project task assignee
   * PATCH /project-task-assignee/:id
   * @param {string} id - Project task assignee ID
   * @param {Object} projectTaskAssignee - Project task assignee data
   * @returns {Promise<UpdateProjectTaskAssigneeResponse>}
   */
  async update(id, projectTaskAssignee) {
    const response = await this.getClient().patch(`/project-task-assignee/${id}`, projectTaskAssignee);
    return response.data;
  }

  /**
   * Delete project task assignee
   * DELETE /project-task-assignee/:id
   * @param {string} id - Project task assignee ID
   * @returns {Promise<void>}
   */
  async delete(id) {
    await this.getClient().delete(`/project-task-assignee/${id}`);
  }
}

export default ProjectTaskAssigneeClient;
