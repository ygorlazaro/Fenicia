import CIcon from "@coreui/icons-react";
import { cilLayers } from "@coreui/icons/dist/esm/free/cil-layers";
import { cilPuzzle } from "@coreui/icons/dist/esm/free/cil-puzzle";
import { CCard, CCardBody, CCardHeader, CCol, CListGroup, CListGroupItem, CRow } from "@coreui/react";
import { CChartDoughnut } from "@coreui/react-chartjs";
import getStyle from "@coreui/utils/dist/esm/getStyle";
import { useTranslation } from "react-i18next";
import { InventoryStockValueByCategory } from "../../../../types/basic/inventory/inventory-stock-value-by-category";
import formatCurrency from "../../../../utils/format-currency";
import formatPercentage from "../../../../utils/format-percentage";

interface StockValueByCategoryProps {
    data: InventoryStockValueByCategory[];
}

export function StockValueByCategory({ data }: StockValueByCategoryProps) {
    const { t } = useTranslation();
    return <CRow>
        <CCol md={6}>
            <CCard className="mb-4">
                <CCardHeader className="d-flex align-items-center">
                    <CIcon icon={cilPuzzle} className="me-2" />
                    <strong>{t('inventory.stockValueByCategory')}</strong>
                </CCardHeader>
                <CCardBody>
                    {data.length === 0 ? <p className="text-muted text-center">{t('common.noData')}</p> : <CChartDoughnut data={{
                        labels: data.map(c => c.categoryName),
                        datasets: [{
                            data: data.map(c => c.totalStockValue),
                            backgroundColor: [getStyle('--cui-primary'), getStyle('--cui-success'), getStyle('--cui-info'), getStyle('--cui-warning'), getStyle('--cui-danger'), getStyle('--cui-secondary')]
                        }]
                    }} options={{
                        responsive: true,
                        maintainAspectRatio: true,
                        plugins: {
                            legend: {
                                position: 'bottom'
                            }
                        }
                    }} />}
                </CCardBody>
            </CCard>
        </CCol>
        <CCol md={6}>
            <CCard className="mb-4">
                <CCardHeader className="d-flex align-items-center">
                    <CIcon icon={cilLayers} className="me-2" />
                    <strong>{t('inventory.categoryBreakdown')}</strong>
                </CCardHeader>
                <CCardBody>
                    {data.length === 0 ? <p className="text-muted text-center">{t('common.noData')}</p> : <CListGroup flush>
                        {data.map(category => <CListGroupItem key={category.categoryId} className="d-flex justify-content-between align-items-center">
                            <div>
                                <div className="fw-semibold">{category.categoryName}</div>
                                <small className="text-body-secondary">
                                    {category.productCount} {t('inventory.products')}
                                </small>
                            </div>
                            <div className="text-end">
                                <div className="text-success fw-semibold">
                                    {formatCurrency(category.totalStockValue)}
                                </div>
                                <small className="text-body-secondary">
                                    {formatPercentage(category.percentage)}
                                </small>
                            </div>
                        </CListGroupItem>)}
                    </CListGroup>}
                </CCardBody>
            </CCard>
        </CCol>
    </CRow>;
}
