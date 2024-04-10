# Advent of Code 2023

I'll try and list general approaches used here on the main page.
I haven't tried getting past Day 21 part 2 as it didn't really rely on programming skills in a significant way anymore.

## Day 1: Trebuchet?

<img src="https://th.bing.com/th/id/OIG3.nT8WoCkUn7WwqEbTN0LC?pid=ImgGn" align="left" alt="Borked" height="200" width="300"/>
You are provided with a document with coordinates hidden within strings

This requires some above average parsing skills.
Quite surprising to see something not so straightforward as the first challenge.

<details>
  <summary>
    Approach part 1:
  </summary>
  Part 1 is pretty straightforward parsing. <br>
  You'll need to be able to convert numbers in a string into integers.
</details>

<details>
  <summary>
    Approach part 2:
  </summary>
  The best approach for part 2 I came up with was storing all possible ways of writing a number in a dictionary  
  and just look for the first and last index of each key. <br>
  Store any found numbers in another dictionary which  
  holds the keys and their indices. <br>
  Then determine the numbers with the absolute first and last index and add those to the total answer.
</details>

