# Advent of Code 2023

I'll try and list general approaches used here on the main page.
I haven't tried getting past Day 21 part 2 as it didn't really rely on programming skills in a significant way anymore.

## Day 1: Trebuchet?

<img src="https://th.bing.com/th/id/OIG3.nT8WoCkUn7WwqEbTN0LC?pid=ImgGn" align="left" alt="Elves working on a trebuchet" height="200" width="300"/>
You are provided with a document with coordinates hidden within strings. You need to find the first and last number in the string and add these together. Then add the result of each string together. Pretty straightforward parsing of integers in a string.

<br>
<details>
  <summary>
    Part 2:
  </summary>
  In part 2 the difficulty is ramped up significantly which is a bit surprising for the first challenge of the year. Now you not only need to find integers but also the written out numbers. Same as before find the first and last in each string.

  <br>

  The best approach I came up with for part 2 was storing all possible ways of writing a number (digit and text) in a dictionary  
  and just look for the first and last index of each key. <br>
  Store any found numbers in another dictionary which holds the keys and their indices.  
  Then determine the numbers with the absolute first and last index and add those to the total answer.
</details>

<br><br><br>

## Day 2: Cube Conundrum

<img src="https://th.bing.com/th/id/OIG2.jyPIlxv61FKwd5RahFEa?pid=ImgGn" align="left" alt="Colored Cubes in a leather bag" height="300" width="300"/>

For day 2 the puzzle input is a list of strings representing the result of a random grab of cubes from a bag. There are several grabs per game separated by the `;` character. In part 1 you are asked how many of the games in the input are possible if the bag contained 12 red cubes, 13 blue cubes and 14 green cubes. Then your output needs to be the sum of all the game IDs.

I chose to use a regex to find the numbers in each string. I also used named groups in the regex:

```C#
string pattern = @"(?<blue>\d+ blue)|(?<red>\d+ red)|(?<green>\d+ green)";
```

So each match would give the number of the cubes for that colour.

I kept the number of cubes in a dictionary, then for each string I keep the highest number of each colour in a string specific dictionary. At the end of the string I compare the game dictionary with the bag dictionary and if all the numbers are equal or lower the game is possible.

<details>
  <summary>
  Part 2:
  </summary>
  Now you are asked to find the minimum number of cubes of each colour that could have been in the bag and the game would still have been possible. You are then to multiply those numbers together which will represent the power of that game's set. Your final output is the sum of all the powers.

<br>

  The approach is basically the same here except that you don't need to compare to the bag dictionary anymore, of course. But the regex approach is still valid. Just update the dictionary if the current match is higher than the current value and you'll get the correct result.
</details>

## Day 3: Gear Ratios

<img src="https://th.bing.com/th/id/OIG2.DtgehU7iQSa2CiWxPYS7?w=1024&h=1024&rs=1&pid=ImgDetMain" align="left" alt="mechanism of many gears" height="300" width="300"/>

For day 3 the input is a large grid with numbers, dots and symbols. The dots representing empty space. The challenge is to find all the numbers that have a symbol surrounding it not just next to it horizontally and vertically but also in the diagonal corners. So the outline if you will. Then add them all together.

In my approach I used a regex to find the numbers and another regex to find anything that is not a number or a dot which means it's a symbol.

```C#
string numberPattern = @"(\d+)";
string symbolPattern = @"([^\d.])";
```

If a match is found with the number pattern then find the starting and ending index. Then check the row above, the current row and the row below with the symbol pattern from starting index -1 to ending index +1. If a match is found for a symbol, add the number to a list. Return the sum of the list at the end.

<details>
  <summary>
  Part 2:
  </summary>
In part 2 the rules are entirely different. Now we know that any * is a gear but only if it is surrounded by two separate numbers. A gear ratio is calculated by multiplying those two numbers. Your final output has to be the sum of all these gear ratios.

So obviously my symbol pattern is no longer valid and I am using a pattern that finds * instead:

```C#
string gearPattern = @"(\*)";
```

This is not as straightforward a flipping around of the challenge as it seems at first glance. The number can start further away from the * but its last digit could still be a neighbour of *. So I ended up just checking the entire rows for numbers and checking if any of them had an index that neighbours the *. If there were two number matches then I would add the product of those two numbers to the list and return the sum of the list at the end.
</details>

## Day 4: Scratchcards

<img src="https://th.bing.com/th/id/OIG2.3i7J2ifdnhfPD90FnLd0?pid=ImgGn" align="left" alt="Scratchcard" height="300" width="300"/>

In Day 4 the puzzle input is a list of strings representing scratchcards. Each card has a sequence of numbers that are winning numbers and a sequence of its actual numbers, these sequences are separated by the `|` character. Up to you to find how many matches there are between the two. One match is one point, every match after that doubles the score which makes the max score 16 for a card. Your final output is the sum of all the scores.

The approach is pretty straightforward. Split the string by `:` and `|`. Check the amount of matches in index 1 and index 2. The linq extension method Intersect is very handy here. 

<br><br>

<details>
  <summary>
  Part 2:
  </summary>
For part 2 we need to apply a ridiculous snowball effect. The scratch cards are not winning you points, the reward is an extra copy of the scratch cards below it in the input. Five winning numbers means an extra copy of all five cards following the current one. So you will get dozens of cards which in turn will win hundreds of cards. You need to calculate how many scratch card you will end up with in total.

<br>

I decided to use an array that keeps track of the copies of each card. Use a for loop to add copies for the value of the current index.
</details>

## Day 5: If You Give A Seed A Fertilizer

<img src="https://th.bing.com/th/id/OIG3.aRkqKrK94XeuTvlaijEN?pid=ImgGn" align="left" alt="Elves working in a vegetable garden" height="300" width="300"/>

The difficulty gets ramped up significantly in Day 5. The puzzle input gives you a list of seeds and 7(!) conversion maps. 

