import React from "react";
import { useAppDispatch, useAppSelector } from "../store";
import { setSidebarShow, setSidebarUnfoldable } from "../features/ui/uiSlice";

import CIcon from "@coreui/icons-react";
import { CCloseButton, CSidebar, CSidebarBrand, CSidebarFooter, CSidebarHeader, CSidebarToggler } from "@coreui/react";
import { NavLink } from "react-router-dom";

import { AppSidebarNav } from "./AppSidebarNav";

import { lines } from "../assets/brand/logo";

import navigation from "../_nav";

const AppSidebar = () => {
    const dispatch = useAppDispatch();
    const unfoldable = useAppSelector((state) => state.ui.sidebarUnfoldable);
    const sidebarShow = useAppSelector((state) => state.ui.sidebarShow);

    return (
        <CSidebar
            className="border-end"
            colorScheme="dark"
            position="fixed"
            unfoldable={unfoldable}
            visible={sidebarShow}
            onVisibleChange={(visible) => {
                dispatch(setSidebarShow(visible));
            }}
        >
            <CSidebarHeader className="border-bottom">
                <CSidebarBrand>
                    <NavLink to="/dashboard">
                        <CIcon customClassName="sidebar-brand-full" icon={lines} height={32} />
                    </NavLink>
                </CSidebarBrand>
                <CCloseButton className="d-lg-none" dark onClick={() => dispatch(setSidebarShow(false))} />
            </CSidebarHeader>
            <AppSidebarNav items={navigation} />
            <CSidebarFooter className="border-top d-none d-lg-flex">
                <CSidebarToggler onClick={() => dispatch(setSidebarUnfoldable(!unfoldable))} />
            </CSidebarFooter>
        </CSidebar>
    );
};

export default React.memo(AppSidebar);
