# Day 21: Step Counter

You manage to catch the airship right as it's dropping someone else off on their all-expenses-paid trip to Desert Island! It even helpfully drops you off near the gardener and his massive farm.

"You got the sand flowing again! Great work! Now we just need to wait until we have enough sand to filter the water for Snow Island and we'll have snow again in no time."

While you wait, one of the Elves that works with the gardener heard how good you are at solving problems and would like your help. 

## Rules

He needs to get his steps in for the day, and so he'd like to know which garden plots he can reach with exactly his remaining 64 steps.

He gives you an up-to-date map (your puzzle input) of his starting position `S`, garden plots `.`, and rocks `#`. 

## Examples

```
...........
.....###.#.
.###.##..#.
..#.#...#..
....#.#....
.##..S####.
.##..#...#.
.......##..
.##.#.####.
.##..##.##.
...........
```

#### First Step
The Elf starts at the starting position `S` which **also counts as a garden plot**. Then, he can take one step north, south, east, or west, but only onto tiles that are garden plots. This would allow him to reach any of the tiles marked `O`:

```
...........
.....###.#.
.###.##..#.
..#.#...#..
....#O#....
.##.OS####.
.##..#...#.
.......##..
.##.#.####.
.##..##.##.
...........
```

#### Second Step
Since at this point he could be at either tile marked `O`, his second step would allow him to reach any garden plot that is one step north, south, east, or west of any tile that he could have reached after the first step:

```
...........
.....###.#.
.###.##..#.
..#.#O..#..
....#.#....
.##O.O####.
.##.O#...#.
.......##..
.##.#.####.
.##..##.##.
...........
```

#### Third Step
He could be at any of the tiles marked `O` above, **_including the starting position_** (either by going north-then-south or by going west-then-east).

Which leads to even more possibilities:

```
...........
.....###.#.
.###.##..#.
..#.#.O.#..
...O#O#....
.##.OS####.
.##O.#...#.
....O..##..
.##.#.####.
.##..##.##.
...........
```

#### Six Steps Taken

He could reach any of the garden plots marked `O`:

```
...........
.....###.#.
.###.##.O#.
.O#O#O.O#..
O.O.#.#.O..
.##O.O####.
.##.O#O..#.
.O.O.O.##..
.##.#.####.
.##O.##.##.
...........
```

In this example, if the Elf's goal was to get exactly `6` more steps today, he could use them to reach any of `16` garden plots.

## Challenge 1

Starting from the garden plot marked `S` on your map, how many garden plots could the Elf reach in exactly `64` steps?