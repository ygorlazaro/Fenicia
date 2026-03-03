import { ApiClient } from './client';

const PROJECTS_API_BASE_URL = import.meta.env.VITE_PROJECTS_API_BASE_URL || 'http://localhost:5144';

/**
 * ProjectAttachmentClient - Handles project attachment CRUD operations
 */
export class ProjectAttachmentClient extends ApiClient {
  constructor(baseURL = PROJECTS_API_BASE_URL) {
    super(baseURL);
  }

  /**
   * Get all project attachments with pagination
   * GET /project-attachment?page=1&perPage=10
   * @param {number} page - Page number
   * @param {number} perPage - Items per page
   * @returns {Promise<Pagination<List<GetAllProjectAttachmentResponse>>>}
   */
  async getAll(page = 1, perPage = 10) {
    const response = await this.getClient().get('/project-attachment', {
      params: { page, perPage }
    });

    return response.data;
  }

  /**
   * Get project attachment by ID
   * GET /project-attachment/:id
   * @param {string} id - Project attachment ID
   * @returns {Promise<GetProjectAttachmentByIdResponse>}
   */
  async getById(id) {
    const response = await this.getClient().get(`/project-attachment/${id}`);
    return response.data;
  }

  /**
   * Create new project attachment
   * POST /project-attachment
   * @param {Object} projectAttachment - Project attachment data
   * @returns {Promise<AddProjectAttachmentResponse>}
   */
  async create(projectAttachment) {
    const response = await this.getClient().post('/project-attachment', projectAttachment);
    return response.data;
  }

  /**
   * Update project attachment
   * PATCH /project-attachment/:id
   * @param {string} id - Project attachment ID
   * @param {Object} projectAttachment - Project attachment data
   * @returns {Promise<UpdateProjectAttachmentResponse>}
   */
  async update(id, projectAttachment) {
    const response = await this.getClient().patch(`/project-attachment/${id}`, projectAttachment);
    return response.data;
  }

  /**
   * Delete project attachment
   * DELETE /project-attachment/:id
   * @param {string} id - Project attachment ID
   * @returns {Promise<void>}
   */
  async delete(id) {
    await this.getClient().delete(`/project-attachment/${id}`);
  }
}

export default ProjectAttachmentClient;
