'use client'

import { NinjaSideBar } from "@/components/NinjaSidebar";

export default function Dashboard() {
    return (
        <div>
            <NinjaSideBar />
            {/* <div className="flex-1 p-4 overflow-y-auto">
                <div className="grid grid-rows-[20px_1fr_20px] items-center justify-items-center min-h-screen p-8 pb-20 gap-16 sm:p-20 font-[family-name:var(--font-geist-sans)]">
                    <main className="flex flex-col gap-[32px] row-start-2 items-center sm:items-start">
                        <div className="flex flex-col gap-4">
                            Dashboard
                        </div>
                    </main>
                </div>
            </div> */}
        </div>
    );
};
