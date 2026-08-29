import { CCol, CRow, CTabPane } from "@coreui/react";
import { t } from "i18next";
import { useEffect, useState } from "react";
import { FeniciaInput } from "../../../../components/fenicia/fenicia-input";
import { FeniciaSelect } from "../../../../components/fenicia/fenicia-select";
import { BasicDataSourceClient } from "../../../../services/basic/basic-datasource-client";
import { CreateOrderCommand, DataSourceItem } from "../../../../types/basic/product-category/add-product-category-command";

interface OrderModalHeaderTabProps {
    visible: boolean;
    onChange: (order: CreateOrderCommand) => void;
    value: CreateOrderCommand;
}

const dataSourceClient = new BasicDataSourceClient();

export default function OrderModalHeaderTab({ visible, onChange, value }: OrderModalHeaderTabProps) {
    const [customers, setCustomers] = useState<DataSourceItem[]>([]);
    const [employees, setEmployees] = useState<DataSourceItem[]>([]);
    const status = [
        { id: "Pending", name: t("orders.statusValues.pending") },
        { id: "Approved", name: t("orders.statusValues.approved") },
        { id: "Cancelled", name: t("orders.statusValues.cancelled") }
    ];
    const [order, setOrder] = useState<CreateOrderCommand>({
        customerId: value.customerId || "",
        saleDate: new Date().toISOString().split("T")[0],
        status: value.status || "Pending",
        employeeId: value.employeeId || "",
        details: value.details || [],
        paymentMethod: value.paymentMethod || "CreditCard",
        notes: value.notes || ""
    });

    useEffect(() => {
        loadCustomers();
        loadEmployees();
    }, []);

    useEffect(() => {
        onChange(order);
    }, [order, onChange]);

    const loadCustomers = async () => {
        try {
            const response = await dataSourceClient.getCustomers();
            const data = Array.isArray(response) ? response : [];
            setCustomers(data);
        } catch (err) {
            console.error("Failed to load customers:", err);
        }
    };

    const loadEmployees = async () => {
        try {
            const response = await dataSourceClient.getEmployees();
            const data = Array.isArray(response) ? response : [];
            setEmployees(data);
        } catch (err) {
            console.error("Failed to load employees:", err);
        }
    };

    const handlerOrder = (field: string, value: string) => {
        setOrder((prevOrder) => ({
            ...prevOrder,
            [field]: value
        }));
    };

    return (
        <CTabPane visible={visible}>
            <CRow>
                <CCol md={6}>
                    <FeniciaSelect label={t("orders.customer")} value={order.customerId} data={customers} onChange={(e) => handlerOrder("customerId", e.target.value)} id="customer" required />
                </CCol>
                <CCol md={6}>
                    <FeniciaInput label={t("orders.date")} type="date" value={order.saleDate} onChange={(e) => handlerOrder("saleDate", e.target.value)} id="saleDate" required />
                </CCol>
                <CCol md={12}>
                    <FeniciaSelect label={t("orders.statusLabel")} value={order.status} data={status} onChange={(e) => handlerOrder("status", e.target.value)} id="status" required />
                </CCol>
                <CCol md={12}>
                    <FeniciaSelect label={t("orders.employee")} value={order.employeeId} data={employees} onChange={(e) => handlerOrder("employeeId", e.target.value)} id="employee" />
                </CCol>
            </CRow>
        </CTabPane>
    );
}
