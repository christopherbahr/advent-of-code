# advent-of-code
Advent of code repo

This is a complete set of solutions to Advent of Code 2021 and 2022.

It has a small helper suite designod to make the kinds of problems we see in AOC a bit easier. These are mostly convenience functions rather than full algorithms (there is no Djikstra or A* here, though maybe there will be eventually).

## Performance Goals
I like to play advent of code in 2 phases. First trying to solve the problem as quickly as possible, then later omtimizing to try to get each day to run in under 1s on whatever computer I'm running.

Particularly in 2021 I've taken good notes of all the various things I thought of and tried to get good performance. Sometimes I'll leave an initial implementation in the repo for posterity if I end up taking a completely different approach.

## Setup
Input files should be called `{DayNumber}.in` and placed in a folder called `inputs/{yearNumber}`. 

## Working On Problems
To work on a problem copy the input to a file called `wip.in` in the root folder and develop in WorkInProgress.cs. Once the problem is solved copy the day into wherever it belongs.

## Run Types
There is a toggle in Program.cs `timedRun`. When false it will only run `WorkInProgress.cs`, when true it will run every day from every year and time each one. If you don't have the input files (which are not checked in at the request of the advent of code maintainer) you can add a cookie from your logged in browser to the CookieStr variable and it will download the input for each file.

There is also an `iterCount` variable that can be used to run `WorkInProgress` many times to get the average time.