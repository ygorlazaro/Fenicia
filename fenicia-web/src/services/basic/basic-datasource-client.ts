import { AxiosResponse } from "axios";
import { FinancialDashboard } from "../../types/basic/dashboard/financial-dashboard";
import type { DataSourceItem } from "../../types/basic/product-category/add-product-category-command";
import { ApiClient } from "../api-client";
import { BASIC_API_BASE_URL } from "./basic-product-client";

/**
 * Basic DataSource Client - Handles datasource lookups
 */

export class BasicDataSourceClient extends ApiClient {
    constructor(baseURL: string = BASIC_API_BASE_URL) {
        super(baseURL);
    }

    async getCustomers(): Promise<DataSourceItem[]> {
        const response = await this.getClient().get("/datasource/customer");
        return (response as AxiosResponse).data;
    }

    async getProducts(): Promise<DataSourceItem[]> {
        const response = await this.getClient().get("/datasource/product");
        return (response as AxiosResponse).data;
    }

    async getSuppliers(): Promise<DataSourceItem[]> {
        const response = await this.getClient().get("/datasource/supplier");
        return (response as AxiosResponse).data;
    }

    async getProductCategories(): Promise<DataSourceItem[]> {
        const response = await this.getClient().get("/datasource/productcategory");
        return (response as AxiosResponse).data;
    }

    async getPositions(): Promise<DataSourceItem[]> {
        const response = await this.getClient().get("/datasource/position");
        return (response as AxiosResponse).data;
    }

    async getEmployees(): Promise<DataSourceItem[]> {
        const response = await this.getClient().get("/datasource/employee");
        return (response as AxiosResponse).data;
    }

    async getFinancialDashboard(days?: number): Promise<FinancialDashboard> {
        const params: any = {};
        if (days !== undefined) params.days = days;

        const response = await this.getClient().get("/dashboard/financial", { params });
        return (response as AxiosResponse).data;
    }
}
