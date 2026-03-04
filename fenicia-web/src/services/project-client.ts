import { ApiClient } from './client';
import { AxiosResponse } from 'axios';

const PROJECTS_API_BASE_URL = import.meta.env.VITE_PROJECTS_API_BASE_URL || 'http://localhost:5004/api';

/**
 * ProjectClient - Handles project CRUD operations
 */
export class ProjectClient extends ApiClient {
  constructor(baseURL: string = PROJECTS_API_BASE_URL) {
    super(baseURL);
  }

  /**
   * Get all projects with pagination
   * GET /project?page=1&perPage=10
   * @param {number} page - Page number
   * @param {number} perPage - Items per page
   * @returns {Promise<any>}
   */
  async getAll(page: number = 1, perPage: number = 10): Promise<any> {
    const response = await this.getClient().get('/project', {
      params: { page, perPage }
    });

    return (response as AxiosResponse).data;
  }

  /**
   * Get project by ID
   * GET /project/:id
   * @param {string} id - Project ID
   * @returns {Promise<any>}
   */
  async getById(id: string): Promise<any> {
    const response = await this.getClient().get(`/project/${id}`);
    return (response as AxiosResponse).data;
  }

  /**
   * Create new project
   * POST /project
   * @param {Object} project - Project data
   * @returns {Promise<any>}
   */
  async create(project: any): Promise<any> {
    const response = await this.getClient().post('/project', project);
    return (response as AxiosResponse).data;
  }

  /**
   * Update project
   * PATCH /project/:id
   * @param {string} id - Project ID
   * @param {Object} project - Project data
   * @returns {Promise<any>}
   */
  async update(id: string, project: any): Promise<any> {
    const response = await this.getClient().patch(`/project/${id}`, project);
    return (response as AxiosResponse).data;
  }

  /**
   * Delete project
   * DELETE /project/:id
   * @param {string} id - Project ID
   * @returns {Promise<void>}
   */
  async delete(id: string): Promise<void> {
    await this.getClient().delete(`/project/${id}`);
  }
}

export default ProjectClient;
