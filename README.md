# QnA-app

My practice on **Carl Rippon**'s book *ASP.NET Core 5 and React 2nd edition*, where I build **Q&amp;A** app

## Used Technologies

- .NET 5 (C# 9)
- ASP.NET Core
- Swagger for API documentation
- React 17
- Typescript 4
- WebSurge for load testing
- Auth0 for providng authentication and authorization services

## Project's Current State

- 2 projects
  - **React** App (with **Typescript**) as a frontend
    - Configuring Development settings (Typescript, Linting with ESLint, Code Formatting with Prettier)
    - Adding **Emotion** (CSS-in-JS Library) for styling components
    - Adding **React Router** to implement client-side pages and the navigation between them
    - Adding **React Hook Form** to reduce boilerplate code for implementing forms
      - Implementing *validation* and *submitting* logic
    - ~~Adding **Redux** to handle complex state scenarios robustly~~
      - ~~Implementing a *Redux store* with a state containing unanswered questions, searched questions, and the question being viewed~~
      - ~~Interacting with the store in the home, search, and question pages using *dispatches* and *selectors*.~~
    - Interacting with the API
      - Using **fetch** to interact with *unauthenticated* REST API endpoints
        - Getting unanswered questions from the REST API
        - Extracting out a generic HTTP http function
        - Getting a question from the REST API
        - Searching questions with the REST API
      - Interacting with Auth0 from the frontend using the **standard Auth0 JavaScript library**
        - Creating the sign-in and sign-out *routes*
        - Implementing a *central authentication context*
        - Implementing the sign-in and sigin-out *processes*
      - Controlling authenticated options
        - Displaying the relevant options in the header
        - Only allowing authenticated users to ask/answer a question
      - Using **fetch** to interact with *authenticated* REST API endpoints
        - Posting a question/answer to the REST API
      - Aborting data fetching if the user navigates away from the page while the data is still being fetched
  - **ASP.NET Core** as a backend
    - Implementing the database
      - Starting initially with 2 tables (Question, and Answer)
      - Creating *store procedures* to interact with the *database tables*.
    - Adding **Dapper ORM** for mapping SQL queries output to C# objects, thus creating a data access layer using *repository design pattern*, which will be used by the API
      - Adding **DbUp** for managing database migrations using *embedded SQL scripts*.
    - Creating **REST API** endpoints
      - Injecting the data repository into the *API controller*
      - Implementing a range of controller *action methods* that handle different *HTTP request methods* returning appropriate *responses*.
      - *Validating requests* to make sure the data is valid before it reaches the data repository
      - *Ensuring* we aren't asking for unnecessary information in the API requests,  preventing potential *security issues* and improving the *usability* of the API as well
    - Improving performance and scalability
      - Reducing the number of *database calls* to improve performance using Dapper's *multi-mapping* and *multi-result* features
      - Implementing *data paging* for requesting less data
      - Making API controllers *asynchronous* achieve scalability benefits
      - Implementing *caching* to reduce the number of expensive database calls
    - Securing the Backend
      - Setting up and Configuring **Auth0** with our ASP.NET backend
      - Protecting endpoints using *Authorize attribute* and *custom authorization policies*
      - Adding **CORS** to let our React app run at certain origins (domains) so that it has permission to access certain resources on a Our REST API at a different origin.
