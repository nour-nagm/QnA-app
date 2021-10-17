# QnA-app
My practice on **Carl Rippon**'s book *ASP.NET Core 5 and React 2nd edition*, where I build **Q&amp;A** app 

## Used Technologies
- .NET 5 (C# 9) 
- ASP.NET Core
- React 17
- Typescript 4

## Current State
- 2 projects
  - React App (with Typescript) as a frontend
    - Configuring Development settings (Typescript, Linting with ESLint, Code Formatting with Prettier)
    - Adding **Emotion** (CSS-in-JS Library) for styling components
    - Adding **React Router** to implement client-side pages and the navigation between them
    - Adding **React Hook Form** to reduce boilerplate code for implementing forms
      - Implementing *validation* and *submitting* logic    
    - Adding **Redux** to handle complex state scenarios robustly
      - Implementing a *Redux store* with a state containing unanswered questions, searched questions, and the question being viewed
      - Interacting with the store in the home, search, and question pages using *dispatches* and *selectors*. 
  - ASP.NET Core as a backend
      - Implementing the database
        - Starting initially with 2 tables (Question, and Answer)
        - Creating store procedures to interact with the database tables.
      - Adding **Dapper ORM** for mapping SQL queries output to C# objects, thus creating a data access layer using *repository design pattern*, which will be used by the API
      - Adding **DbUp** for managing database migrations using *embedded SQL scripts*.
      - Creating **REST API** endpoints
        - Injecting the data repository into the *API controller* 
        - Implementing a range of controller *action methods* that handle different *HTTP request methods* returning appropriate *responses*.
        -  *Validating* requests so that we can be sure the data is valid before it reaches the data repository
        - Ensuring we aren't asking for unnecessary information in the API requests,  preventing potential *security issues* and improving the *usability* of the API 

