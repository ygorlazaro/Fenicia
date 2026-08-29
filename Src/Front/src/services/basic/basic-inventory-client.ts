import { AxiosResponse } from "axios";
import { InventoryHealth } from "../../types/basic/inventory/inventory-health";
import { ApiClient } from "../api-client.ts";

const BASIC_API_BASE_URL = import.meta.env.VITE_BASIC_API_BASE_URL || "http://localhost:5083";

/**
 * Inventory Client - Handles inventory dashboard operations
 */
export class BasicInventoryClient extends ApiClient {
    constructor(baseURL: string = BASIC_API_BASE_URL) {
        super(baseURL);
    }

    async getDashboard(): Promise<any> {
        const response = await this.getClient().get("/inventory/dashboard");
        return (response as AxiosResponse).data;
    }

    async getInventory(page: number = 1, perPage: number = 10): Promise<any> {
        const response = await this.getClient().get("/inventory", { params: { page, perPage } });
        return (response as AxiosResponse).data;
    }

    async getByProduct(productId: string): Promise<any> {
        const response = await this.getClient().get(`/inventory/product/${productId}`);
        return (response as AxiosResponse).data;
    }

    async getByCategory(categoryId: string): Promise<any> {
        const response = await this.getClient().get(`/inventory/category/${categoryId}`);
        return (response as AxiosResponse).data;
    }

    async getInventoryHealth(zeroMovementDays?: number, overstockMultiplier?: number): Promise<InventoryHealth> {
        const params: any = {};
        if (zeroMovementDays !== undefined) params.zeroMovementDays = zeroMovementDays;
        if (overstockMultiplier !== undefined) params.overstockMultiplier = overstockMultiplier;

        const response = await this.getClient().get("/inventory/health", { params });
        return (response as AxiosResponse).data;
    }
}

export default BasicInventoryClient;
