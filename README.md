# Multi-Tier UAV Search and Rescue

**Table of Contents**

<!-- TOC -->

- [Multi-Tier UAV Search and Rescue](#multi-tier-uav-search-and-rescue)
    - [Abstract](#abstract)
    - [Introduction](#introduction)
    - [Background](#background)
    - [Environment Setup](#environment-setup)
    - [Drone Classification](#drone-classification)
        - [Stage 1 Drone](#stage-1-drone)
        - [Stage 2 Drone](#stage-2-drone)
        - [Stage 3 Drone](#stage-3-drone)
    - [Drone Functions](#drone-functions)
        - [Stage 1 - Domain Mapping](#stage-1---domain-mapping)
        - [Stage 2 - Object Classification](#stage-2---object-classification)
        - [Stage 3 - Target Approach](#stage-3---target-approach)
    - [Methodology](#methodology)
    - [Drone Controller](#drone-controller)
    - [Challenges](#challenges)
    - [Results](#results)
    - [Conclusion](#conclusion)
    - [Designer Details](#designer-details)
    - [License](#license)

<!-- /TOC -->

## Abstract

**The use of unmanned aerial vehicles (UAVs) for search and rescue (SAR) operations has become increasingly popular in recent years. One of the critical challenges in SAR is to plan an efficient and safe path for the UAV to search the area for the target. This project develops a path-planning algorithm for UAVs in SAR operations. The algorithm considers various factors such as the terrain, the location of obstacles, and the target's last known position to generate a path that minimizes the search time and maximizes the chances of finding the target. The proposed algorithm has been tested in a simulated environment, and the results have been evaluated based on the path's efficiency and accuracy in finding the target. This project aims to contribute to the development of more efficient and effective SAR operations using UAVs, which can ultimately save lives and reduce the time and cost associated with traditional SAR methods.**

## Introduction

In emergencies such as natural disasters. accidents, and other emergencies time is of the essence, which makes search and rescue operations crucial. Traditional SAR methods are usually time-consuming, labor and resource intensive, and the sites are often dangerous for human rescuers. In recent years UAVs or drones, have emerged as a promising technology for SAR operations. Drones offer several advantages over traditional search and rescue methods, such as the ability to cover large areas quickly, access hard-to-reach areas, and capture real-time video footage of the affected areas. Additionally, drones can provide critical information to rescue teams, such as the location of survivors, hazardous conditions, and the overall situation in the affected area. These UAVs are generally manned by operators far away from the site keeping humans safe, but their vigilance is always required.

In this project we have made this task autonomous, our multi-tiered drone network can perform these SAR operations with higher accuracy keeping in mind the complexity of rescue sites and the time pressure. We aim to leverage the unique capabilities of our fleet of drones, including the Planner Drone, the Search Drone, and the Retrieval Drone, to perform effective search and rescue operations. By using the A* algorithm for motion planning, we can optimize the performance of our fleet of drones and improve the efficiency of search and rescue operations. Overall, the use of drones in search and rescue operations can save time, resources, and potentially, lives. This project aims to demonstrate the potential of drones in real-world applications and advance the field of search and rescue operations.

![Gem](./Docs/Images/Gem.png)

*Fig.1 Lost Object*

## Background

The use of unmanned aerial vehicles (UAVs) in search and rescue missions has become increasingly popular due to their ability to quickly access hard-to-reach areas and provide real-time data for situational awareness. However, effective path-planning algorithms and aerodynamic control strategies are critical for the successful deployment of UAVs in such missions.

To address this need, researchers have proposed various approaches. Sun et al. (2019) proposed an improved genetic algorithm for the path planning of UAVs in search and rescue missions, while Zhang et al. (2020) developed an efficient path-planning approach using Voronoi diagrams and A*. On the other hand, Huang et al. (2009) discussed the aerodynamics and control of autonomous quadrotor helicopters in aggressive maneuvers, and Falanga et al. (2017) presented a system for aggressive quadrotor flight through narrow gaps with onboard sensing and computing using active vision.

Together, these studies highlight the potential of UAVs in search and rescue missions and underscore the need for further research in developing efficient and effective path-planning algorithms as well as aerodynamics and control strategies. By enhancing the capabilities of UAVs, such as onboard sensing and computing, it is possible to increase their usefulness for more challenging maneuvers. This background section will review the current state of research in these areas and present the objectives of this study in developing novel path planning and control strategies for UAVs in search and rescue missions.


## Environment Setup

The simulation environment was constructed using Unity.  The environment consists of a field to be populated with obstacles, a launch platform, and three drones.  At run start the field is populated with trees with tetromino-shaped canopies and thin trunks. The obstacles are populated randomly to represent a relatively dense forest. A target object, the king's diamond, is also placed randomly in the forest.


![Orthographic View](./Docs/Images/Orthographic%20View.png)

*Fig.2 Orthographic View*

![Perspective View](./Docs/Images/Perspective%20View.png)

*Fig.3 Perspective View*

## Drone Classification

The drones are divided into three categories with distinct purposes and capabilities.

![Drone Top View](./Docs/Images/Drone%20Top%20View.png)

*Fig.4 Drone Top View*

### Stage 1 Drone

The first stage drone works as a domain mapping tool to map the area and provide initial data for the next two drones. The properties of this drone are:

- Large Battery Life
- Perfect Perception Stack
  - To get an overview of the entire forest from the center, the first drone is equipped with a hemispherical stereo pair at the bottom. 
  - This provides a large field of view and maximum coverage of the forest from the top.
- Low Payload Capacity
- Long Flight Times
- Slow Flight Speed
- Large Form Factor

![Stage 1 Drone](./Docs/Images/Drone%201.png)

*Fig.5 Stage 1 Drone*

### Stage 2 Drone

The second stage drone builds upon the information provided by the first to scout the area and attempt to locate the target object. The properties of this drone are:

- Small Battery Life
- Low-Quality Perception Stack
  - The primary function of the perception stack on this drone is to detect obstacles directly in front of it to plan maneuverability around them using a LiDAR with 3x3 resolution mounted on top of the drone to detect the distance at which the obstacle is present.
  - Downward-facing camera with basic object recognition capabilities
- No Payload Capacity
- Small Flight Times
- High Speeds
- Mini Form Factor

![Stage 2 Drone](./Docs/Images/Drone%202.png)

*Fig.6 Stage 2 Drone*

### Stage 3 Drone

The final stage drone acts upon the totality of information gathered and seeks to find and return the target object. The properties of this drone are:

- Moderate Battery Life
- Perfect Perception Stack
  - A high-resolution monocular camera paired with a 3D LiDAR is mounted on the third drone. 
  - The high-frequency camera stream and the robust LiDAR data together help the drone maneuver through the obstacles quickly yet efficiently.
- High Payload Capacity
- Small Flight Times
- Moderate Speeds
- Moderate Form Factor

![Stage 3 Drone](./Docs/Images/Drone%203.png)

*Fig.7 Stage 3 Drone*

## Drone Functions

### Stage 1 - Domain Mapping

The first stage, also known as domain mapping, will involve deploying a drone at an altitude higher than the obstacles to create a comprehensive map of the terrain. The drone's primary function will be to generate an initial grid map that does not differentiate between objects based on their height or whether they are obstacles or targets. The drone's battery life will not be a limiting factor, as its mission duration will be brief.

### Stage 2 - Object Classification

The second stage, known as object classification, will involve deploying a drone at obstacle height to distinguish between obstacles and targets detected on the initial grid map generated in Stage 1. The drone's mission will be to map as many objects as possible within the constraints of its limited battery life. To achieve this objective, the drone's trajectory will need to be optimized to detect the maximum number of obstacles possible, given the limited energy resources available. It should be noted that mapping the entire workspace will not be feasible due to battery constraints, underscoring the importance of optimizing the drone's flight path to maximize obstacle detection.  Additional complexity can be added in this stage and the next by adding additional targets that must be identified and tracked.  The current implementation uses only one target but this can be easily scaled pending successful initial implementation.

### Stage 3 - Target Approach

The third stage, referred to as the target approach, will involve deploying a small and agile drone to navigate to the identified target(s) while avoiding any obstacles detected in Stages 1 and 2. The drone will be flown at or above obstacle height, depending on the path planning algorithm's optimization objectives. The primary goal of the path planning algorithm will be to strike a balance between the drone's speed and its battery life, as flying above obstacles would enable faster navigation but require additional battery power that may be constrained due to the drone's limited capacity. The drone will aim to reach all identified targets while avoiding all detected obstacles, necessitating the use of a sophisticated navigation system capable of adapting to changing environments and obstacles.

## Methodology

Our approach to choosing drones has been based on leveraging the strengths of various types of drones. The type 1 drone has the benefit of high battery life at the side effects of slower speed and low payload capacity. We have used this drone to hover at high altitudes and act as eyes for the remaining drone where it perceives the density of the forest and provides a heatmap indicating where the highest concentration of obstacles are for the stage 2 drone to prioritize searching.

The second drone has high rpm motors which help the UAV to achieve high speeds with a small size at the cost of lower battery life and low payload. This drone is equipped with a 3x3 LiDar Array for obstacle avoidance and a low-quality downwards-facing camera for the perception of lost objects making it an ideal candidate to traverse through forests and go through tight spaces.

The third drone has high payload capacity with a moderate form factor at the cost of smaller battery life which makes it an ideal candidate to retrieve objects from a forest in a tight duration.

## Drone Controller

For this project, we have implemented a sliding mode controller with a boundary layer. This provides robust control by eliminating non-linearities from the dynamic model of the system. Adding a boundary layer constraint to the controller ensures that the discontinuity in the control law across the sliding manifold is taken care of. This is called the chattering problem. Control over all 6 DoFs of the drones is possible using this controller. We have implemented this controller on a drone model in Gazebo. To test and demonstrate the controller's performance, trajectories were generated using quintic polynomials. The drone was tested to track multiple trajectories as shown below:

## Challenges

- One of the main challenges in this project is establishing effective communication between drones at different stages. 
- The accuracy of the planner drones is limited to the field visibility and the range and availability of the onboard sensors.
- The next challenge was considering the battery life of each drone. Particularly, the second drone might lack battery life to locate the diamond.
- Thus, due to the battery life considerations, the optimization effort of the second drone presented some challenges.


## Results
Using the three stages, the King's diamond was successfully found and retrieved from the magical forest of Anuba. The three drones work in unison to achieve this as follows:
- Drone 1 takes the skies and gets an environment overview for the other stages.
- Drone 2 quickly maneuvered through the forest by dodging obstacles.
- Drone 3 reaches and retrieves the diamond using the exact location provided by Larry.

All three stages were completed and the diamond was retrieved successfully with each drone keeping its battery life and payload constraints in check.  

![Results](./Docs/Images/trajectory.png)

*Fig.8 Trajectory Tracing using the drone*

[![Animation Video](https://img.youtube.com/vi/ElPuGj_Wy78/0.jpg)](https://www.youtube.com/watch?v=ElPuGj_Wy78)

*Video: Final Animation*

## Conclusion

In conclusion, this project aimed to develop an efficient and effective path-planning algorithm for unmanned aerial vehicles (UAVs) in search and rescue (SAR) operations. The proposed algorithm considers various factors such as terrain, location of obstacles, and the target's last known position to generate a path that minimizes search time and maximizes the chances of finding the target. The project simulated the environment using Unity, and the results were evaluated based on the path's efficiency and accuracy in finding the target. This project contributes to the development of more efficient and effective SAR operations using UAVs, which can ultimately save lives and reduce the time and cost associated with traditional SAR methods. Further research in developing efficient and effective path-planning algorithms and aerodynamic control strategies for UAVs in SAR missions is needed to enhance their capabilities and usefulness in challenging maneuvers.

## Designer Details

- Designed for:
  - Worcester Polytechnic Institute
  - RBE550-S23-S01: Motion Planning (Online) - Final Project
- Designed by:
  - [Parth Patel](mailto:parth.pmech@gmail.com)

## License

This project is licensed under [GNU General Public License v3.0](https://www.gnu.org/licenses/gpl-3.0.en.html) (see [LICENSE.md](LICENSE.md)).

Copyright 2023 Parth Patel

Licensed under the GNU General Public License, Version 3.0 (the "License"); you may not use this file except in compliance with the License.

You may obtain a copy of the License at

_https://www.gnu.org/licenses/gpl-3.0.en.html_

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
