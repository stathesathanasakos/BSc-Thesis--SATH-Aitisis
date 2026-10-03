# Bachelor's Thesis: SATH- Aitisis

SATH-Aitisis is a web application, developed as a part of  My Bachelor's Thesis in the Department of Informatics, University of Piraeus. It is designed to help citizens easily submit applications for benefits and services with end-to-end tracking from submission to completion. The system aims to digitize, streamline, and accelerate the submission, review, and protocol assignment processes for citizen applications (such as social benefits and certificates). Built with ASP.NET Core 8.0, EF Core and ASP.NET Identity, it integrates external payment and AI APIs. 4-layer Clean Architecture (Domain, App, Infrastructure, Web), SOLID principles (e.g SRP & OCP) & Design Patterns (e.g Adapter & Cache-Aside).

---

## 🎯 Key Features & User Roles

The platform provides dedicated workflows for three primary user roles:

* **Citizens:**
  * Submit online applications across various categories and services.
  * Upload required supporting documents.
  * Pay application fees online via a secure external checkout gateway (where applicable).
  * Track application status and review decisions directly through a personal dashboard.

* **Employees (Reviewers):**
  * Review citizen applications and inspect submitted details and documents.
  * Utilize an integrated **AI Assistant** to get automated application summaries, key metadata, and verification checklists.
  * Approve or reject submissions, with automatic generation and assignment of an official Protocol Number upon approval.

* **Administrators:**
  * Manage users, roles, and system access policies.
  * Configure application types, eligibility rules, required documents, and associated fees.

---

## 🛠️ Technology Stack

* **Framework:** ASP.NET Core 8.0 MVC
* **Security & Auth:** ASP.NET Core Identity (Role-Based Access Control)
* **Data Access & ORM:** Entity Framework Core
* **Database:** MySQL 
* **Payment Processing:** Stripe Checkout (Test Environment)
* **AI & LLM Services:** Groq Cloud API (openai/gpt-oss-120b Model)
* **Presentation / UI:** Razor Pages/Views (`.cshtml`), Bootstrap 5
