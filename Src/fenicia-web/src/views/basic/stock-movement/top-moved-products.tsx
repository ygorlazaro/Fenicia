import { cilLayers, cilSpeedometer } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CCard, CCardBody, CCardHeader, CCol, CRow, CTable, CTableBody, CTableDataCell, CTableHead, CTableHeaderCell, CTableRow } from "@coreui/react";
import { useTranslation } from "react-i18next";
import { StockTurnover } from "../../../types/basic/stock-movement/stock-turnover";
import { TopMovedProduct } from "../../../types/basic/stock-movement/top-moved-product";
import { formatNumber } from "../../../utils/format-number";

interface TopMovedProductsProps {
    openProductModal: (productId: string, event: React.MouseEvent<HTMLAnchorElement>) => void;
    topMovedProducts: TopMovedProduct[] | null;
    turnoverRates: StockTurnover[] | null;
}

export default function TopMovedProducts({ openProductModal, topMovedProducts, turnoverRates }: TopMovedProductsProps) {
    const { t } = useTranslation();

    const getTurnoverBadgeColor = (classification: string) => {
        switch (classification.toLowerCase()) {
            case "high":
                return "success";
            case "medium":
                return "warning";
            case "low":
                return "orange";
            default:
                return "danger";
        }
    };

    return (
        <CRow xs={{ gutter: 4 }}>
            <CCol md={6}>
                <CCard className="mb-4">
                    <CCardHeader className="d-flex align-items-center">
                        <CIcon icon={cilLayers} className="me-2" size="lg" />
                        <strong>{t("stockMovement.topMovedProducts")}</strong>
                    </CCardHeader>
                    <CCardBody>
                        {!topMovedProducts || topMovedProducts.length === 0 ? (
                            <p className="text-muted text-center">{t("common.noData")}</p>
                        ) : (
                            <CTable hover responsive>
                                <CTableHead>
                                    <CTableRow>
                                        <CTableHeaderCell>{t("stockMovement.product")}</CTableHeaderCell>
                                        <CTableHeaderCell className="text-end">{t("stockMovement.totalMoved")}</CTableHeaderCell>
                                        <CTableHeaderCell className="text-end">{t("stockMovement.movements")}</CTableHeaderCell>
                                    </CTableRow>
                                </CTableHead>
                                <CTableBody>
                                    {topMovedProducts?.map((product) => (
                                        <CTableRow key={product.productId}>
                                            <CTableDataCell>
                                                <a href={`/basic/products?id=${product.productId}`} onClick={(e) => openProductModal(product.productId, e)} className="text-decoration-none">
                                                    <div className="fw-semibold">{product.productName}</div>
                                                </a>
                                                <small className="text-body-secondary">{product.categoryName}</small>
                                            </CTableDataCell>
                                            <CTableDataCell className="text-end">
                                                <strong>{formatNumber(product.totalMoved)}</strong>
                                            </CTableDataCell>
                                            <CTableDataCell className="text-end">
                                                <span className="badge bg-secondary">{product.movementCount}</span>
                                            </CTableDataCell>
                                        </CTableRow>
                                    ))}
                                </CTableBody>
                            </CTable>
                        )}
                    </CCardBody>
                </CCard>
            </CCol>

            <CCol md={6}>
                <CCard className="mb-4">
                    <CCardHeader className="d-flex align-items-center">
                        <CIcon icon={cilSpeedometer} className="me-2" size="lg" />
                        <strong>{t("stockMovement.turnoverRates")}</strong>
                    </CCardHeader>
                    <CCardBody>
                        {!turnoverRates || turnoverRates.length === 0 ? (
                            <p className="text-muted text-center">{t("common.noData")}</p>
                        ) : (
                            <CTable hover responsive>
                                <CTableHead>
                                    <CTableRow>
                                        <CTableHeaderCell>{t("stockMovement.product")}</CTableHeaderCell>
                                        <CTableHeaderCell className="text-end">{t("stockMovement.rate")}</CTableHeaderCell>
                                        <CTableHeaderCell className="text-center">{t("stockMovement.classification")}</CTableHeaderCell>
                                    </CTableRow>
                                </CTableHead>
                                <CTableBody>
                                    {turnoverRates?.map((item) => (
                                        <CTableRow key={item.productId}>
                                            <CTableDataCell>
                                                <a href={`/basic/products?id=${item.productId}`} onClick={(e) => openProductModal(item.productId, e)} className="text-decoration-none">
                                                    <div className="fw-semibold">{item.productName}</div>
                                                </a>
                                                <small className="text-body-secondary">{item.categoryName}</small>
                                            </CTableDataCell>
                                            <CTableDataCell className="text-end">
                                                <strong>{item.turnoverRate.toFixed(2)}x</strong>
                                            </CTableDataCell>
                                            <CTableDataCell className="text-center">
                                                <span className={`badge bg-${getTurnoverBadgeColor(item.turnoverClassification)}`}>{t(`stockMovement.${item.turnoverClassification.toLowerCase().replace(/\s+/g, "")}`)}</span>
                                            </CTableDataCell>
                                        </CTableRow>
                                    ))}
                                </CTableBody>
                            </CTable>
                        )}
                    </CCardBody>
                </CCard>
            </CCol>
        </CRow>
    );
}
