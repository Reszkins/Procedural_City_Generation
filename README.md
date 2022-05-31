# Procedural_City_Generation

WORK IN PROGRESS.

Unity application for generating a procedural city map.
So far I have implemented generation of city boundaries and basic road generation.

Results of the work so far:

City based on voronoi diagram with 9 iterations of lloyd algorithm and roads generated by recursive land splitting of every voronoi diagram element:

![city 2 lloyd](https://user-images.githubusercontent.com/73691017/170682444-f1962a05-d62d-4696-aff7-6197fd3cfccd.png)

City with the same parametres but with 2 iterations of lloyd algorithm:

![tru 2 lloyd](https://user-images.githubusercontent.com/73691017/170682043-bebbfad7-fe6d-4a63-994a-97751fc79f49.png)

City boundaries based on voronoi and main roads generated with MST:

![city mst](https://user-images.githubusercontent.com/73691017/170682209-3c94e926-feb6-4e69-b670-8bd2b4cd3274.png)

Large city:

![big city](https://user-images.githubusercontent.com/73691017/170682301-08c41c3c-b229-4dc2-afb4-78da4cad50cf.png)

TODO:
- road generation using tensor field
- building generation
