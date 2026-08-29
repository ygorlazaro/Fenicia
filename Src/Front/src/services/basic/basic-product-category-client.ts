import { AxiosResponse } from "axios";
import { IPagination } from "../../types/index.ts";
import { AddProductCategoryCommand, AddProductCategoryResponse, UpdateProductCategoryCommand, UpdateProductCategoryResponse } from "../../types/basic/product-category/add-product-category-command";
import { GetAllProductCategoryResponse } from "../../types/basic/product-category/get-all-product-category-response";
import { GetProductCategoryByIdResponse } from "../../types/basic/product-category/get-product-category-by-id-response";
import { ApiClient } from "../api-client.ts";
import { BASIC_API_BASE_URL } from "./basic-product-client";

/**
 * Basic Product Category Client - Handles product category CRUD operations
 */

export class BasicProductCategoryClient extends ApiClient {
    constructor(baseURL: string = BASIC_API_BASE_URL) {
        super(baseURL);
    }

    async getAll(page: number = 1, perPage: number = 10): Promise<IPagination<GetAllProductCategoryResponse>> {
        const response = await this.getClient().get("/productcategory", { params: { page, perPage } });
        return (response as AxiosResponse).data;
    }

    async getById(id: string): Promise<GetProductCategoryByIdResponse> {
        const response = await this.getClient().get(`/productcategory/${id}`);
        return (response as AxiosResponse).data;
    }

    async create(category: AddProductCategoryCommand): Promise<AddProductCategoryResponse> {
        const response = await this.getClient().post("/productcategory", category);
        return (response as AxiosResponse).data;
    }

    async update(id: string, category: UpdateProductCategoryCommand): Promise<UpdateProductCategoryResponse> {
        const response = await this.getClient().patch(`/productcategory/${id}`, category);
        return (response as AxiosResponse).data;
    }

    async delete(id: string): Promise<void> {
        await this.getClient().delete(`/productcategory/${id}`);
    }
}
