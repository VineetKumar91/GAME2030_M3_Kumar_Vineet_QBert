# GAME2030_M3_Kumar_Vineet_QBert
 Final milestone for Game Production 4 - Qbert
•	There is an executable build under ../GAME2030_M3_Kumar_Vineet_QBert/_EXECUTABLE-BUILD/
•	A GDD has also been included which contains some basic information about the Milestone 3 build. The same can be found below.
 
All the requirements of the milestone 3 have been met:
1.	A successful win condition with appropriate SFX and VFX (victory music and pyramid flashing colors) with 1000 bonus points awarded. If high score is there, the player will be prompted for initials in the next scene
2.	A Game Over scene is implemented when the level is completed, or player’s lives are over. If in game over state, the player has achieved high score, then the player will be prompted for the initials, else just a Game Over with continue button is displayed.
3.	In case the player beats any of the top 10 scores of Qbert’s Leaderboard, the player will be prompted to enter high score regardless of win/loss of the gameplay.
4.	Qbert will die with swear cloud in case of collision with coily, purple ball or red ball.
In case the player dies due to jumping off the pyramid, appropriate SFX is played (without swear cloud)
5.	Player lives with icons are displayed on the left side of the screen. The player has three chances (current and 2 more as per the lives icons). If the player dies without any player icon left, it will trigger gameover.
6.	If Qbert jumps of the pyramid, he will spawn on the topmost block of the pyramid. If Qbert dies due to collision of enemy, Qbert will spawn on the same block where he died.
7.	Coily can be defeated by taking the elevator at appropriate time. This will fool coily and he will stop chasing Qbert and attempt to jump off the pyramid before Qbert lands on the top block.
a.	Note: In case Coily has not yet jumped off the pyramid and Qbert lands on the top of the pyramid, Coily will start chasing Qbert again
b.	Note: In case Coily does jump off the pyramid, all the enemies (red balls) will disappear for 5 seconds.
8.	Appropriate SFX as stated in the Milestone requirement pdf have been implemented with additional SFX and VFX for green ball.
9.	Appropriate scoring of change of cube color, green ball, coily defeat, clearing a level with number of elevators remaining has been implemented.
10.	In case the player catches a green ball, it will freeze the enemies and there will be music and background VFX played (as per original Qbert game)
11.	UI functionality is perfectly done.
12.	Leaderboard functionality is implemented correctly where the player can view the top 10 scores SORTED appropriately. The topmost score is highlighted. The leaderboard is saved offline, meaning, quitting the game will not remove the leaderboard data. It will still be reflected when player restarts the game later.
