# Abstract
The "Ground Control" research project of the Swiss National Science Foundation focused on analysing the parameters of the special metal deposition process (3D printers) and optimising its control. To acquire the data of the deposition process, imaging and thermal cameras are used to provide images, pyrometers to measure temperatures, and NC machines to provide machine data (motor positions, laser power, etc.). 
The objective of the semester project was to develop a new desktop application able to displaying the acquired data in an organised manner. The data is acquired from another application, and organised by "experiment", which can contain one or more "depositions" (3D prints), which in turn contain the data (images, temperatures, NC machine data).

Using the new application in offline mode, the user must be able to select and open one or more "experiments" or "depositions", and visualise and compare the data, or export them in compressed format. It shall be possible to delete both experiments and depositions, and to add comments and images of the 3D printed specimen. 
In online mode, the operator must be able to create and associate to each experiment a set of setups, where each device has its own setup. The devices used are the following: cameras, pyrometers, database and software version, GCode for the NC machine, type of NC machine.

# Delivered project
After many changes the delivered project handles only the "offline" datas. That mean that the software manages the datas which are saved in a text file by the sensors. The main reason that bring us to that solution is that the database was also growning and changing during the whole development of the software.
Anyway the team that commission this project was very enthusiatic about the final work that I give them, because the software help them with the data management.

The documentation language is italian.
