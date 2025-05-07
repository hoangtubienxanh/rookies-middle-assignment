"use client"

import React from "react"
import ReactDOM from "react-dom/client"
import { RouterProvider } from "@tanstack/react-router"
import { router } from "@/lib/router"
import "@/app/globals.css"

// Make sure the router is ready before rendering
async function prepareAndRender() {
  // Initialize the router
  await router.load()

  ReactDOM.createRoot(document.getElementById("root")!).render(
    <React.StrictMode>
      <RouterProvider router={router} />
    </React.StrictMode>,
  )
}

prepareAndRender()
