import { cilPeople, cilWarning } from "@coreui/icons"
import CIcon from "@coreui/icons-react"
import { CCard, CCardBody, CCardHeader, CProgress, CTable, CTableBody, CTableDataCell, CTableHead, CTableHeaderCell, CTableRow } from "@coreui/react"
import { t } from "i18next"
import { LowStockItem } from "../../../types/basic/inventory/low-stock-item"
import formatCurrency from "../../../utils/format-currency"

interface LowStockItemsTableProps {
    items: LowStockItem[];
    onProductClick: (productId: string, event: React.MouseEvent) => void;
    onCategoryClick: (categoryId: string, event: React.MouseEvent) => void;
}

export default function LowStockItemsTable({ items, onProductClick, onCategoryClick }: LowStockItemsTableProps) {
    const getStockLevel = (quantity: number) => {
        if (quantity === 0) return { color: 'danger', label: t('inventory.outOfStock') }
        if (quantity < 10) return { color: 'warning', label: t('inventory.lowStock') }
        return { color: 'success', label: t('inventory.inStock') }
    }

    return (<CCard>
        <CCardHeader className="d-flex align-items-center">
            <CIcon icon={cilWarning} className="me-2 text-warning" size="lg" />
            <strong>{t('inventory.lowStockItems')}</strong>
        </CCardHeader>
        <CCardBody>
            {items.length === 0 ? (
                <div className="text-center py-4">
                    <p className="text-muted">{t('common.noData')}</p>
                </div>
            ) : (
                <CTable align="middle" className="mb-0 border" hover responsive>
                    <CTableHead>
                        <CTableRow>
                            <CTableHeaderCell className="bg-body-tertiary">
                                {t('inventory.productName')}
                            </CTableHeaderCell>
                            <CTableHeaderCell className="bg-body-tertiary text-center">
                                {t('inventory.category')}
                            </CTableHeaderCell>
                            <CTableHeaderCell className="bg-body-tertiary text-center">
                                {t('inventory.quantity')}
                            </CTableHeaderCell>
                            <CTableHeaderCell className="bg-body-tertiary text-end">
                                {t('inventory.costPrice')}
                            </CTableHeaderCell>
                            <CTableHeaderCell className="bg-body-tertiary text-end">
                                {t('inventory.salesPrice')}
                            </CTableHeaderCell>
                            <CTableHeaderCell className="bg-body-tertiary text-center">
                                {t('inventory.status')}
                            </CTableHeaderCell>
                        </CTableRow>
                    </CTableHead>
                    <CTableBody>
                        {items.map((item) => {
                            const stockLevel = getStockLevel(item.quantity)
                            return (
                                <CTableRow key={item.id}>
                                    <CTableDataCell>
                                        <a href={`/basic/products?id=${item.id}`} onClick={(e) => onProductClick(item.id, e)} className="text-decoration-none">
                                            <div className="fw-semibold">{item.name}</div>
                                        </a>
                                    </CTableDataCell>
                                    <CTableDataCell className="text-center">
                                        <a href={`/basic/product-categories?id=${item.categoryId}`} onClick={(e) => onCategoryClick(item.categoryId, e)} className="text-decoration-none">
                                            {item.categoryName}
                                        </a>
                                    </CTableDataCell>
                                    <CTableDataCell className="text-center">
                                        <div className="d-flex align-items-center justify-content-center">
                                            <CIcon icon={cilPeople} className="me-2" size="sm" />
                                            <span className="fw-semibold">{item.quantity}</span>
                                        </div>
                                    </CTableDataCell>
                                    <CTableDataCell className="text-end">
                                        {formatCurrency(item.costPrice ?? 0)}
                                    </CTableDataCell>
                                    <CTableDataCell className="text-end">
                                        {formatCurrency(item.salesPrice)}
                                    </CTableDataCell>
                                    <CTableDataCell className="text-center">
                                        <CProgress
                                            thin
                                            color={stockLevel.color}
                                            value={Math.min(item.quantity * 10, 100)}
                                            style={{ width: '60px', display: 'inline-block' }}
                                        />
                                        <div className="small text-body-secondary mt-1">{stockLevel.label}</div>
                                    </CTableDataCell>
                                </CTableRow>
                            )
                        })}
                    </CTableBody>
                </CTable>
            )}
        </CCardBody>
    </CCard>
    )
}   
