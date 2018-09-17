class DrawScene
{
    constructor(scene)
    {
        this.scene = scene;
    }

    DrawRobot(x, y, z)
    {
        var geometry = new THREE.BoxGeometry(x, y, z);
        var cubeMaterials = [
            new THREE.MeshBasicMaterial({ map: new THREE.TextureLoader().load("Textures/robot_side.png"), side: THREE.DoubleSide }), //LEFT
            new THREE.MeshBasicMaterial({ map: new THREE.TextureLoader().load("Textures/robot_side.png"), side: THREE.DoubleSide }), //RIGHT
            new THREE.MeshBasicMaterial({ map: new THREE.TextureLoader().load("Textures/robot_top.png"), side: THREE.DoubleSide }), //TOP
            new THREE.MeshBasicMaterial({ map: new THREE.TextureLoader().load("Textures/robot_bottom.png"), side: THREE.DoubleSide }), //BOTTOM
            new THREE.MeshBasicMaterial({ map: new THREE.TextureLoader().load("Textures/robot_front.png"), side: THREE.DoubleSide }), //FRONT
            new THREE.MeshBasicMaterial({ map: new THREE.TextureLoader().load("Textures/robot_front.png"), side: THREE.DoubleSide }), //BACK
        ];
        var material = new THREE.MeshFaceMaterial(cubeMaterials);
        var robot = new THREE.Mesh(geometry, material);
        robot.position.y = 0.15;

        var group = new THREE.Group();
        group.add(robot);

        return group;

    }
}