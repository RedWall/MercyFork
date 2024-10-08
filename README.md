![MercyForkGHBanner](https://github.com/RedWall/MercyFork/assets/2397638/9e670639-4492-4309-beda-e8e6af9acc93)

[![Build MercyFork](https://github.com/RedWall/MercyFork/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/RedWall/MercyFork/actions/workflows/build.yml)

# What is this?
Mercy Fork is your go-to tool for rescuing abandoned or unmaintained repositories! 🛠️✨

Ever stumbled upon a great project only to find it’s been left in the dust? Mercy Fork swoops in to save the day by helping you find popular and well-maintained forks. If no good alternatives exist, you can easily create your own _Mercy Fork_ 😉 and keep the project alive.

Join the mission to give these projects a second chance! 🚀

# Technology Stack
- .NET 9 
- ASP.NET Core (Blazor and Web API)
- Aspire

# Getting Started
Once you have cloned the repository, you can get a set of test data by running the following PowerShell script:
```bash 
./TestData/GetReposFromGithub.ps1
```

This will download a set of repository data from GitHub to the `TestData` directory. You can then run `dotnet build` or use Visual Studio to build the solution.

Running the solution using `dotnet run --project MercyFork.AppHost` or Visual Studio will start the Aspire host project. You can then click on the appropriate link to open the Web App. Navigate to the "Repos" page to see the list of repositories.
