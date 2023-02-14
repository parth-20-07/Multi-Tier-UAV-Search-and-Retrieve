# Setup Crazyflie 2.0 Quadrotor in Gazebo

To set up the Crazyflie 2.0 quadrotor in Gazebo, we need to install additional ROS dependencies for building packages as below:

```sh
sudo apt update
```

```sh
sudo apt install ros-noetic-joy ros-noetic-octomap-ros ros-noetic-mavlink
```

```sh
sudo apt install ros-noetic-octomap-mapping ros-noetic-control-toolbox
```

```sh
sudo apt install python3-vcstool python3-catkin-tools protobuf-compiler libgoogle-glog-dev
```

```sh
rosdep update
sudo apt-get install ros-noetic-ros libgoogle-glog-dev
```

We are now ready to create a new ROS workspace and download the ROS packages for the robot:

```sh
mkdir -p rbe_project/src
cd rbe_project/src
catkin_init_workspace # initialize your catkin workspace
cd ..
```

```sh
catkin init
catkin build -j1
cd src
git clone -b dev/ros-noetic https://github.com/gsilano/CrazyS.git
git clone -b med18_gazebo9 https://github.com/gsilano/mav_comm.git
```

_Note: a new ROS workspace is needed for the project, because the CrazyS Gazebo package is built using the `catkin build` tool[^1], instead of `catkin_make`._

[^1]: *-j1 in catkin build is for safety so it does not cause you computer to hang. It makes your code to build on just one core. Slow but ensures it compiles without issues*

We need to build the project workspace using `python_catkin_tools` , therefore we need to configure it:

```sh
cd ..
rosdep install --from-paths src -i
rosdep update
catkin config --cmake-args -DCMAKE_BUILD_TYPE=Release -DCATKIN_ENABLE_TESTING=False
catkin build -j1
```

_This is gonna take a lot of time. Like a real lot. So maybe make yourself a cup of coffee meanwhile? If you don't like coffee, I don't know your motivation to live :worried:_

Do not forget to add sourcing to your `.bashrc` file:

```sh
source devel/setup.bash
```

With all dependencies ready, we can build the ROS package by the following commands:

```sh
catkin build -j1
```

To spawn the quadrotor in Gazebo, we can run the following launch file:

```sh
roslaunch rotors_gazebo crazyflie2_without_controller.launch
```

Congrats, All the setup is done for the simulation to start. You can start writing your own algorithm if you want to!! :smiley:
