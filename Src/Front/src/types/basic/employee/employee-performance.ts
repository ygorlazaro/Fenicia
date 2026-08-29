import { EmployeeOrderCount } from "./employee-order-count";
import { EmployeePerformanceSummary } from "./employee-performance-summary";
import { EmployeeSales } from "./employee-sales";
import { TopPerformer } from "./top-performer";

export interface EmployeePerformance {
    summary: EmployeePerformanceSummary;
    salesByEmployee: EmployeeSales[];
    ordersByEmployee: EmployeeOrderCount[];
    topPerformers: TopPerformer[];
}
