import React from "react";

const AuthLogin = React.lazy(() => import("./views/auth/login"));
const AuthRegister = React.lazy(() => import("./views/auth/register"));
const AuthCompany = React.lazy(() => import("./views/auth/company"));
const ForgotPassword = React.lazy(() => import("./views/auth/forgot-password"));
const ResetPassword = React.lazy(() => import("./views/auth/reset-password"));
const Profile = React.lazy(() => import("./template/profile"));
const Subscription = React.lazy(() => import("./views/subscription"));
const EmployeeList = React.lazy(() => import("./views/basic/employee"));
const PositionList = React.lazy(() => import("./views/basic/position"));
const Customers = React.lazy(() => import("./views/basic/customer"));
const Suppliers = React.lazy(() => import("./views/basic/supplier"));
const ProductCategories = React.lazy(() => import("./views/basic/product-category"));
const Products = React.lazy(() => import("./views/basic/product"));
const Inventory = React.lazy(() => import("./views/basic/inventory"));
const StockMovement = React.lazy(() => import("./views/basic/stock-movement"));
const OrderDetail = React.lazy(() => import("./views/basic/order-detail"));
const NotificationList = React.lazy(() => import("./views/auth/notifications/notification-list"));
const NotificationDetail = React.lazy(() => import("./views/auth/notifications/notification-detail"));
const ProjectList = React.lazy(() => import("./template/project"));
const ProjectStatusList = React.lazy(() => import("./template/project/status"));
const ProjectTaskList = React.lazy(() => import("./template/project/task"));
const ProjectSubtaskList = React.lazy(() => import("./template/project/subtask"));
const ProjectCommentList = React.lazy(() => import("./template/project/comment"));
const ProjectAttachmentList = React.lazy(() => import("./template/project/attachment"));
const ProjectTaskAssigneeList = React.lazy(() => import("./template/project/task-assignee"));
const Dashboard = React.lazy(() => import("./views/dashboard"));
const Orders = React.lazy(() => import("./views/basic/order"));

const routes = [
    { path: "/", exact: true, name: "Home" },
    { path: "/dashboard", name: "Dashboard", element: Dashboard },
    {
        path: "/notifications",
        name: "Notifications",
        element: NotificationList,
        exact: true
    },
    {
        path: "/notifications/:id",
        name: "Notification Details",
        element: NotificationDetail
    },
    { path: "/auth/login", name: "Login", element: AuthLogin },
    { path: "/auth/register", name: "Register", element: AuthRegister },
    { path: "/auth/company", name: "Company Selection", element: AuthCompany },
    {
        path: "/auth/forgot-password",
        name: "Forgot Password",
        element: ForgotPassword
    },
    {
        path: "/auth/reset-password",
        name: "Reset Password",
        element: ResetPassword
    },
    { path: "/profile", name: "Profile", element: Profile },
    { path: "/subscription", name: "Subscription", element: Subscription },
    { path: "/basic/employees", name: "Employees", element: EmployeeList },
    { path: "/basic/positions", name: "Positions", element: PositionList },
    { path: "/basic/customers", name: "Customers", element: Customers },
    { path: "/basic/suppliers", name: "Suppliers", element: Suppliers },
    {
        path: "/basic/product-categories",
        name: "Product Categories",
        element: ProductCategories
    },
    { path: "/basic/products", name: "Products", element: Products },
    { path: "/basic/inventory", name: "Inventory", element: Inventory },
    {
        path: "/basic/stock-movements",
        name: "Stock Movements",
        element: StockMovement
    },
    { path: "/basic/orders", name: "Orders", element: Orders },
    { path: "/basic/order/:id", name: "Order Details", element: OrderDetail },
    { path: "/project/projects", name: "Projects", element: ProjectList },
    {
        path: "/project/status",
        name: "Project Status",
        element: ProjectStatusList
    },
    { path: "/project/tasks", name: "Project Tasks", element: ProjectTaskList },
    {
        path: "/project/subtasks",
        name: "Project Subtasks",
        element: ProjectSubtaskList
    },
    {
        path: "/project/comments",
        name: "Project Comments",
        element: ProjectCommentList
    },
    {
        path: "/project/attachments",
        name: "Project Attachments",
        element: ProjectAttachmentList
    },
    {
        path: "/project/task-assignees",
        name: "Project Task Assignees",
        element: ProjectTaskAssigneeList
    }
];

export default routes;
