import { ApiClient } from './client';
import { AxiosResponse } from 'axios';

const PROJECTS_API_BASE_URL = import.meta.env.VITE_PROJECTS_API_BASE_URL || 'http://localhost:5144';

/**
 * ProjectSubtaskClient - Handles project subtask CRUD operations
 */
export class ProjectSubtaskClient extends ApiClient {
  constructor(baseURL: string = PROJECTS_API_BASE_URL) {
    super(baseURL);
  }

  async getAll(page: number = 1, perPage: number = 10): Promise<any> {
    const response = await this.getClient().get('/project-subtask', { params: { page, perPage } });
    return (response as AxiosResponse).data;
  }

  async getById(id: string): Promise<any> {
    const response = await this.getClient().get(`/project-subtask/${id}`);
    return (response as AxiosResponse).data;
  }

  async create(data: any): Promise<any> {
    const response = await this.getClient().post('/project-subtask', data);
    return (response as AxiosResponse).data;
  }

  async update(id: string, data: any): Promise<any> {
    const response = await this.getClient().patch(`/project-subtask/${id}`, data);
    return (response as AxiosResponse).data;
  }

  async delete(id: string): Promise<void> {
    await this.getClient().delete(`/project-subtask/${id}`);
  }
}

export default ProjectSubtaskClient;
