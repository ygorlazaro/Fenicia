import { AxiosResponse } from "axios";
import { IPagination } from "../../types/index.ts";
import { Notification } from "../../types/notification/notification";
import { AuthClient } from "../auth/auth-client";

/**
 * NotificationClient - Handles notification operations from the Auth microservice
 */
class NotificationClient extends AuthClient {
    async getAll(page: number = 1, perPage: number = 100): Promise<IPagination<Notification>> {
        const response = await this.getClient().get("/notification", { params: { page, perPage } });
        return (response as AxiosResponse).data;
    }

    async getById(id: string): Promise<Notification | null> {
        const response = await this.getClient().get(`/notification/${id}`);
        return (response as AxiosResponse).data;
    }

    async getRecent(limit: number = 5): Promise<Notification[]> {
        const paginated = await this.getAll(1, limit);
        return paginated.data;
    }

    async markAsRead(id: string): Promise<void> {
        await this.getClient().patch(`/notification/${id}/read`);
    }
}

export default NotificationClient;
