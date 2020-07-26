# HackerNews 

This project tries to address the requirements of a coding test. 
To run the application, one should start the "Api.exe" executable located in "Api\bin\Release\netcoreapp3.1", then open a browser and access the "http://localhost:5000/api/hackernews" url.

Considerations:
- This is just a "proof of concept" test so everything is kept in the same solution for the sake of simplicity. In a real-world scenario a different approach would be used regarding projects, folders and files structures.
- For the Hacker News endpoints constants were used, again in a real-world scenario this endpoints should be place in a config file. 
- MemoryCache was used in order to cache Hacker News API results. SlidingExpiration and AbsoluteExpiration were placed in a config file but further testing is needed in order to find the "sweet spot" for the expiration values.
 