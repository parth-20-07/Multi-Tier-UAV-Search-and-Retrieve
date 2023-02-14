# Install ROS1 Noetic

1. Setup your sources.list

    Setup your computer to accept software from packages.ros.org.

    ```sh
    sudo sh -c 'echo "deb http://packages.ros.org/ros/ubuntu $(lsb_release -sc) main" > /etc/apt/sources.list.d/ros-latest.list'
    ```

2. Set up your keys

    ```sh
    sudo apt install curl # if you haven't already installed curl
    ```

    ```sh
    curl -s https://raw.githubusercontent.com/ros/rosdistro/master/ros.asc | sudo apt-key add -
    ```

3. Installation

    First, make sure your Debian package index is up-to-date:

    ```sh
    sudo apt update
    ```

    Desktop-Full Install: (Recommended) : Everything in Desktop plus 2D/3D simulators and 2D/3D perception packages

    ```sh
    sudo apt install ros-noetic-desktop-full
    ```

4. Environment setup

    You must source this script in every bash terminal you use ROS in.

    ```sh
    source /opt/ros/noetic/setup.bash
    ```

    It can be convenient to automatically source this script every time a new shell is launched. These commands will do that for you.

    ```sh
    echo "source /opt/ros/noetic/setup.bash" >> ~/.bashrc
    source ~/.bashrc
    ```

5. Dependencies for building packages

    Up to now you have installed what you need to run the core ROS packages. To create and manage your own ROS workspaces, there are various tools and requirements that are distributed separately. For example, [rosinstall](https://wiki.ros.org/rosinstall) is a frequently used command-line tool that enables you to easily download many source trees for ROS packages with one command.
    
    To install this tool and other dependencies for building ROS packages, run:

    ```sh
    sudo apt install python3-rosdep python3-rosinstall python3-rosinstall-generator python3-wstool build-essential
    ```

    Initialize rosdep
    
    Before you can use many ROS tools, you will need to initialize rosdep. rosdep enables you to easily install system dependencies for source you want to compile and is required to run some core components in ROS. If you have not yet installed rosdep, do so as follows.

    ```sh
    sudo apt install python3-rosdep
    ```

    With the following, you can initialize rosdep.

    ```sh
    sudo rosdep init
    rosdep update
    ```

6. Install the Control and Effort Dependencies for Gazebo

    ```sh
    sudo apt-get update
    sudo apt-get upgrade
    sudo apt update
    ```

    ```sh
    sudo apt-get install ros-noetic-ros-control ros-noetic-ros-controllers
    sudo apt-get install ros-noetic-gazebo-ros-pkgs ros-noetic-gazebo-ros-control
    ```
