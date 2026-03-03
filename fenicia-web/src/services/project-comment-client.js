import { ApiClient } from './client';

const PROJECTS_API_BASE_URL = import.meta.env.VITE_PROJECTS_API_BASE_URL || 'http://localhost:5144';

/**
 * ProjectCommentClient - Handles project comment CRUD operations
 */
export class ProjectCommentClient extends ApiClient {
  constructor(baseURL = PROJECTS_API_BASE_URL) {
    super(baseURL);
  }

  /**
   * Get all project comments with pagination
   * GET /project-comment?page=1&perPage=10
   * @param {number} page - Page number
   * @param {number} perPage - Items per page
   * @returns {Promise<Pagination<List<GetAllProjectCommentResponse>>>}
   */
  async getAll(page = 1, perPage = 10) {
    const response = await this.getClient().get('/project-comment', {
      params: { page, perPage }
    });

    return response.data;
  }

  /**
   * Get project comment by ID
   * GET /project-comment/:id
   * @param {string} id - Project comment ID
   * @returns {Promise<GetProjectCommentByIdResponse>}
   */
  async getById(id) {
    const response = await this.getClient().get(`/project-comment/${id}`);
    return response.data;
  }

  /**
   * Create new project comment
   * POST /project-comment
   * @param {Object} projectComment - Project comment data
   * @returns {Promise<AddProjectCommentResponse>}
   */
  async create(projectComment) {
    const response = await this.getClient().post('/project-comment', projectComment);
    return response.data;
  }

  /**
   * Update project comment
   * PATCH /project-comment/:id
   * @param {string} id - Project comment ID
   * @param {Object} projectComment - Project comment data
   * @returns {Promise<UpdateProjectCommentResponse>}
   */
  async update(id, projectComment) {
    const response = await this.getClient().patch(`/project-comment/${id}`, projectComment);
    return response.data;
  }

  /**
   * Delete project comment
   * DELETE /project-comment/:id
   * @param {string} id - Project comment ID
   * @returns {Promise<void>}
   */
  async delete(id) {
    await this.getClient().delete(`/project-comment/${id}`);
  }
}

export default ProjectCommentClient;
