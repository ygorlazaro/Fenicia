import { cilPlus, cilTrash } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CButton, CCol, CRow, CTable, CTableBody, CTableDataCell, CTableFoot, CTableHead, CTableHeaderCell, CTableRow, CTabPane } from "@coreui/react";
import { t } from "i18next";
import { useEffect, useState } from "react";
import { FeniciaInput } from "../../../../components/fenicia/fenicia-input";
import { FeniciaSelect } from "../../../../components/fenicia/fenicia-select";
import { BasicDataSourceClient } from "../../../../services/basic/basic-datasource-client";
import { BasicProductClient } from "../../../../services/basic/basic-product-client";
import { CreateOrderCommand, DataSourceItem } from "../../../../types/basic/product-category/add-product-category-command";
import formatCurrency from "../../../../utils/format-currency";

interface OrderDetailTabProps {
    visible: boolean;
    setError: (message: string) => void;
    onChange: (order: CreateOrderCommand) => void;
    value: CreateOrderCommand;
}

const dataSourceClient = new BasicDataSourceClient();
const productClient = new BasicProductClient();

export default function OrderDetailTab({ visible, onChange, setError, value }: OrderDetailTabProps) {
    const [products, setProducts] = useState<DataSourceItem[]>([]);
    const [selectedProduct, setSelectedProduct] = useState("");
    const [quantity, setQuantity] = useState(1);
    const [price, setPrice] = useState(0);
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
        loadProducts();
    }, []);

    const loadProducts = async () => {
        try {
            const response = await dataSourceClient.getProducts();
            const data = Array.isArray(response) ? response : [];
            setProducts(data);
        } catch (err) {
            console.error("Failed to load products:", err);
        }
    };

    const handleProductChange = (productId: string) => {
        setSelectedProduct(productId);

        async function fetchProductPrice() {
            try {
                const product = await productClient.getById(productId);
                setPrice(product.salesPrice || 0);
            } catch (err) {
                console.error("Failed to fetch product price:", err);
                setPrice(0);
            }
        }
        fetchProductPrice();
    };

    const handleAddItem = () => {
        if (!selectedProduct || quantity <= 0 || price <= 0) {
            setError(t("common.requiredField"));
            return;
        }

        const product = products.find((p) => p.id === selectedProduct);
        if (!product) return;

        const existingItem = order.details.find((item) => item.productId === selectedProduct);

        if (existingItem) {
            order.details = order.details.map((item) => (item.productId === selectedProduct ? { ...item, quantity: item.quantity + Number(quantity) } : item));
        } else {
            order.details.push({
                productId: selectedProduct,
                price: Number(price),
                quantity: Number(quantity)
            });
        }

        setSelectedProduct("");
        setQuantity(1);
        setPrice(0);
    };
    const handleRemoveItem = (productId: string) => {
        order.details = order.details.filter((item) => item.productId !== productId);
    };

    return (
        <CTabPane visible={visible}>
            <h6 className="mb-3">{t("orders.addItems")}</h6>

            <CRow className="mb-4">
                <CCol md={5}>
                    <FeniciaSelect label={t("products.title")} value={selectedProduct} data={products} onChange={(e) => handleProductChange(e.target.value)} id="product" required />
                </CCol>
                <CCol md={2}>
                    <FeniciaInput label={t("products.price")} type="number" min={0.01} step={0.01} id="price" value={price} onChange={(e) => setPrice(~~e.target.value)} required />
                </CCol>
                <CCol md={2}>
                    <FeniciaInput label={t("orders.quantity")} type="number" min={1} step={1} id="quantity" value={quantity} onChange={(e) => setQuantity(~~e.target.value)} required />
                </CCol>
                <CCol md={3} className="d-flex align-items-end">
                    <CButton color="primary" onClick={handleAddItem} disabled={!selectedProduct}>
                        <CIcon icon={cilPlus} className="me-2" />
                        {t("common.add")}
                    </CButton>
                </CCol>
            </CRow>

            <h6 className="mb-3">{t("orders.items")}</h6>

            {order.details.length === 0 ? (
                <div className="text-center py-4">
                    <p className="text-muted">{t("orders.noItems")}</p>
                </div>
            ) : (
                <CTable hover responsive>
                    <CTableHead>
                        <CTableRow>
                            <CTableHeaderCell>{t("products.name")}</CTableHeaderCell>
                            <CTableHeaderCell className="text-end">{t("products.price")}</CTableHeaderCell>
                            <CTableHeaderCell className="text-end">{t("orders.quantity")}</CTableHeaderCell>
                            <CTableHeaderCell className="text-end">{t("orders.subtotal")}</CTableHeaderCell>
                            <CTableHeaderCell className="text-end">{t("common.actions")}</CTableHeaderCell>
                        </CTableRow>
                    </CTableHead>
                    <CTableBody>
                        {order.details.map((item) => (
                            <CTableRow key={item.productId}>
                                {/* <CTableDataCell>{item.productName}</CTableDataCell> */}
                                <CTableDataCell className="text-end">{formatCurrency(item.price)}</CTableDataCell>
                                <CTableDataCell className="text-end">{item.quantity}</CTableDataCell>
                                <CTableDataCell className="text-end">{formatCurrency(item.price * item.quantity)}</CTableDataCell>
                                <CTableDataCell className="text-end">
                                    <CButton color="danger" size="sm" onClick={() => handleRemoveItem(item.productId)}>
                                        <CIcon icon={cilTrash} />
                                    </CButton>
                                </CTableDataCell>
                            </CTableRow>
                        ))}
                    </CTableBody>
                    <CTableFoot>
                        <CTableRow>
                            <CTableHeaderCell colSpan={3} className="text-end">
                                {t("orders.total")}:
                            </CTableHeaderCell>
                            <CTableHeaderCell className="text-end">
                                <strong>{formatCurrency(order.details.reduce((sum, item) => sum + item.price * item.quantity, 0))}</strong>
                            </CTableHeaderCell>
                            <CTableHeaderCell></CTableHeaderCell>
                        </CTableRow>
                    </CTableFoot>
                </CTable>
            )}
        </CTabPane>
    );
}
