export interface LinkModuleRoute { 
    name: string,
    href: string
}

export interface LinkModule { 
    name: string,
    type: number;
    isSelected: boolean;
    routes: LinkModuleRoute[]
}


export const linkModules: LinkModule[] = [{
    name: "Basic",
    isSelected: false,
    type: 1,
    routes: [
        { name: "Customers", href: "/basic/customers" },
        { name: "Suppliers", href: "/basic/suppliers" },
        { name: "Employees", href: "/basic/employees" },
        { name: "Positions", href: "/basic/positions" },
        { name: "Products", href: "/basic/productss" },
        { name: "Categrories", href: "/basic/categories" },
        { name: "Stock Movements", href: "/basic/stockmovement" },
        { name: "Buys", href: "/basic/buys" },
        { name: "Sells", href: "/basic/sells" },
        { name: "Inventory", href: "/basic/inventory" },
    ]
}, {
    name: "Social Network",
    isSelected: false,
    type: 2,
    routes: [
        { name: "Feed", href: "/social/feed" },
        { name: "Profile", href: "/social/profile" },
        { name: "Posts", href: "/social/posts" },
        { name: "Comments", href: "/social/comments" },
        { name: "Likes", href: "/social/likes" },
        { name: "Shares", href: "/social/shares" },
        { name: "Schedules", href: "/social/schedule" },
    ]
},
{
    name: "Projects",
    isSelected: false,
    type: 3,
    routes: [
        { name: "Projects", href: "/projects/projects" },
        { name: "Tasks", href: "/projects/tasks" },
        { name: "Assignments", href: "/projects/assignments" },
        { name: "Reports", href: "/projects/reports" },
        { name: "Status", href: "/projects/status" },
    ]
},
{
    name: "Performance",
    isSelected: false,
    type: 4,
    routes: [
        { name: "Employees", href: "/performance/employees" },
        { name: "Evaluations", href: "/performance/evaluations" },
        { name: "Reports", href: "/performance/reports" },
    ]
}, {
    name: "Accounting",
    isSelected: false,
    type: 5,
    routes: [
        { name: "Accounts", href: "/accounting/accounts" },
        { name: "Transactions", href: "/accounting/transactions" },
        { name: "Reports", href: "/accounting/reports" },
    ],
},
{
    name: "HR",
    isSelected: false,
    type: 6,
    routes: [
        { name: "Employees", href: "/hr/employees" },
        { name: "Departments", href: "/hr/departments" },
        { name: "Positions", href: "/hr/positions" },
        { name: "Reports", href: "/hr/reports" },
        { name: "Documents", href: "/hr/documents", },
        { name: "Applications", href: "/hr/applications" },
        { name: "Evaluations", href: "/hr/evaluations" },
    ]
}, {
    name: "POS",
    isSelected: false,
    type: 7,
    routes: [
        { name: "POS", href: "/pos/pos" },
        { name: "Orders", href: "/pos/orders" },
        { name: "Reports", href: "/pos/reports" },
    ]
}, {
    name: "Contracts",
    isSelected: false,
    type: 8,
    routes: [
        { name: "Contracts", href: "/contracts/contracts" },
        { name: "Reports", href: "/contracts/reports" },
        { name: "Signatures", href: "/contracts/signatures" }
    ]
},
{
    name: "Ecommerce",
    isSelected: false,
    type: 9,
    routes: [
        { name: "Ecommerce", href: "/ecommerce/ecommerce" },
        { name: "Tracking", href: "/ecommerce/tracking" },
        { name: "Reports", href: "/ecommerce/reports" }
    ]
}, {
    name: "Customer Support",
    isSelected: false,
    type: 10,
    routes: [
        { name: "Tickets", href: "/customer-support/tickets" },
        { name: "Reports", href: "/customer-support/reports" },
        { name: "FAQ", href: "/customer-support/faq" }
    ]
}];
