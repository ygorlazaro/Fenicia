import { AppContent, AppFooter, AppHeader, AppSidebar, ChatWidget, PWAInstallPrompt } from "../components/index";

const DefaultLayout = () => {
    return (
        <div>
            <AppSidebar />
            <div className="wrapper d-flex flex-column min-vh-100">
                <AppHeader />
                <div className="body flex-grow-1">
                    <AppContent />
                </div>
                <AppFooter />
                <PWAInstallPrompt />
                <ChatWidget />
            </div>
        </div>
    );
};

export default DefaultLayout;
