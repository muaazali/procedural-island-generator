# Procedural Island Generator - Unity
A low-poly island mesh generator giving you semi-fine grain control over your generations. It doen't necessarily need to be used for islands only and can generate quite a lot of shapes based on your specifications.

## Table of Contents
1. [Overview](#overview)
2. [Examples](#examples)
3. [How to Install](#how-to-install)
4. [How to Use](#how-to-use)
5. [Parameters](#parameters)

# Overview
Mesh generation has always intrigued me so I decided to give it a try while working on my game; Heaven Isles Chronicles. The results have been astonishing and you can see what I was able to generate with a single script after adding a custom shader and throwing some vegetation on it.
![](https://i.imgur.com/i2Skx7p.png)

It doesn't use any complex algorithms, and simply uses basic trigonometry to calcualte points around the cirumference of a circle based on the given input. Then, it applies the requested variations going down the levels and ends up connecting all the vertices to form triangles. Even with complicated shapes, I was able to keep the polygon count lower than 100 for my game's islands (the example screenshot above).

# Examples
I was able to generate a number of different objects but with this tool, the limits are pretty low. You can check out some of my creations:

<details>
<summary>1. Basic Shapes</summary>
<br></br>

![](https://i.imgur.com/a1hveCI.png)
![](https://i.imgur.com/9cZZej7.png)
![](https://i.imgur.com/heTbQ12.png)
</details>

<details>
<summary>2. Some Complex Shapes</summary>
<br></br>

![](https://i.imgur.com/TFaqNUs.png)
![](https://i.imgur.com/KyrYw3O.png)
</details>

<details>
<summary>3. Whole Islands</summary>
<br></br>

![](https://i.imgur.com/lFkxL8b.png)
![](https://i.imgur.com/0XHJBFy.png)
![](https://i.imgur.com/i2Skx7p.png)
</details>


# How to Install
Copy all the files in the repository to your project's `Assets` directory. That's it.

# How to Use
1. Attach the `Island Mesh Generator` component to an empty gameobject.<br></br>![](https://i.imgur.com/l1aZ8IR.png)
2. Select a material for your object. If you ignore this step, the mesh you generate will not be displayed. Select a material first!<br></br>![](https://i.imgur.com/0kveC0l.png)
3. Set up your parameters under the `Levels` dropdown of the `Island Mesh Generator` component. At the very minimum, add two levels.<br></br>![](https://i.imgur.com/kV61eXs.png)
4. Set your `Resolution` parameters to the number of corner points you want in your mesh.<br></br>![](https://i.imgur.com/S36tbDF.png)
5. Press the `Generate Island` button!<br></br>![](https://i.imgur.com/nXd8bGR.pngg)

# Parameters
There are quite a few parameters given to tweak the shape of your generated mesh.
- `Resolution` is the number of corner points. Increasing it will result in smoother shapes depending on the other parameters. Each level comprises of `Resolution` number of points, so the final mesh will have `Levels`*`Resolution`+2 vertices.
> +2 for center points of top and bottom faces.
- `Spacing` defines the distance between each of the points for the current level. Increasing spacing will result in bigger shapes.
- `Height` defines the height for the current level.
- `Max Variance In Spacing` allows you to define the max level of spacing (x and z axes) variation that can be performed on a point in the current level. Bigger variance values will result in more randomness in spacing.
- `Max Variance In Height` allows you to define the max level of vertical (y axis) variation that can be performed on a point in the current level. Bigger variance values will result in more randomness in height.
- `Duplicate Above Vertices` allows you to copy paste the spacing and position of the vertices in the above level. Only the height can be different.

Play around with the parameters to experiment with different shapes!