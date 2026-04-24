import { cilCart, cilUser } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CButton, CForm, CModal, CModalBody, CModalFooter, CModalHeader, CModalTitle, CNav, CNavItem, CNavLink, CTabContent } from "@coreui/react";
import { t } from "i18next";
import { useState } from "react";
import { CreateOrderCommand } from "../../../../types/basic/product-category/add-product-category-command";
import OrderDetailTab from "./order-detail-tab";
import OrderModalHeaderTab from "./order-modal-header-tab";

interface CreateOrderModalProps {
    modalVisible: boolean;
    setModalVisible: (visible: boolean) => void;
    handleSave: (e: React.FormEvent, payload: CreateOrderCommand) => void;
    setError: (message: string) => void;
}

export default function CreateOrderModal({ modalVisible, setModalVisible, handleSave, setError }: CreateOrderModalProps) {
    const [activeTab, setActiveTab] = useState(0);
    const [orderItems, setOrderItems] = useState([]);
    const [order, setOrder] = useState<CreateOrderCommand>({
        customerId: '',
        saleDate: new Date().toISOString().split('T')[0],
        status: 'Pending',
        employeeId: '',
        details: [],
        paymentMethod: 'CreditCard',
        notes: ''
    });

    const onSaving = (e: React.FormEvent) => {

        e.preventDefault();

        if (!order.customerId || !order.saleDate || !order.status) {
            setError(t('common.requiredField'));
            return;
        }

        handleSave(e, order);
    }

    return (<CModal visible={modalVisible} onClose={() => setModalVisible(false)} size="xl">
        <CModalHeader>
            <CModalTitle>{t('orders.new')}</CModalTitle>
        </CModalHeader>
        <CForm onSubmit={onSaving} >
            <CModalBody>
                <CNav variant="tabs">
                    <CNavItem>
                        <CNavLink
                            active={activeTab === 0}
                            onClick={() => setActiveTab(0)}
                            style={{ cursor: 'pointer' }}
                        >
                            <CIcon icon={cilUser} className="me-2" />
                            {t('orders.header')}
                        </CNavLink>
                    </CNavItem>
                    <CNavItem>
                        <CNavLink
                            active={activeTab === 1}
                            onClick={() => setActiveTab(1)}
                            style={{ cursor: 'pointer' }}
                        >
                            <CIcon icon={cilCart} className="me-2" />
                            {t('orders.details')}
                        </CNavLink>
                    </CNavItem>
                </CNav>

                <CTabContent className="mt-3">
                    <OrderModalHeaderTab visible={activeTab == 0} onChange={setOrder} value={order}/>

                    <OrderDetailTab visible={activeTab == 1} value={order} setError={setError} onChange={setOrder} />
                </CTabContent>
            </CModalBody>
            <CModalFooter>
                <CButton
                    color="primary"
                    type="submit"
                >
                    {t('orders.create')}
                </CButton>
            </CModalFooter>
        </CForm>
    </CModal>)
}
