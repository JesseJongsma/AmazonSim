﻿class DrawScene {

    constructor(scene) {
        this.scene = scene;
    }

    //Draw sphere.
    drawSphere(x, y, z, posx, posy, posz, pathPicture = null) {
        var sphereGeometry = new THREE.SphereGeometry(x, y, z);
        var sphereMaterial = new THREE.MeshLambertMaterial({ map: new THREE.TextureLoader().load(pathPicture), side: THREE.DoubleSide });
        var sphere = new THREE.Mesh(sphereGeometry, sphereMaterial);
        sphere.position.set(posx, posy, posz);
        return sphere;
    }

    //Draw plane.
    drawPlane(width, height, x, y, z, setColor) {
        var geometry = new THREE.PlaneGeometry(width, height, 32);
        var material = new THREE.MeshLambertMaterial({ color: setColor, side: THREE.DoubleSide });
        var plane = new THREE.Mesh(geometry, material);
        plane.rotation.x = Math.PI / 2.0;
        plane.position.set(x, y, z);
        return plane;
    }

    //Draw robot.
    drawRobot(x, y, z) {
        var geometry = new THREE.BoxGeometry(x, y, z);
        var cubeMaterials = [
            new THREE.MeshLambertMaterial({ map: new THREE.TextureLoader().load("Textures/robot_side.png"), side: THREE.DoubleSide }), //LEFT
            new THREE.MeshLambertMaterial({ map: new THREE.TextureLoader().load("Textures/robot_side.png"), side: THREE.DoubleSide }), //RIGHT
            new THREE.MeshLambertMaterial({ map: new THREE.TextureLoader().load("Textures/robot_top.png"), side: THREE.DoubleSide }), //TOP
            new THREE.MeshLambertMaterial({ map: new THREE.TextureLoader().load("Textures/robot_bottom.png"), side: THREE.DoubleSide }), //BOTTOM
            new THREE.MeshLambertMaterial({ map: new THREE.TextureLoader().load("Textures/robot_front.png"), side: THREE.DoubleSide }), //FRONT
            new THREE.MeshLambertMaterial({ map: new THREE.TextureLoader().load("Textures/robot_front.png"), side: THREE.DoubleSide }) //BACK
        ];
        var material = new THREE.MeshFaceMaterial(cubeMaterials);
        var robot = new THREE.Mesh(geometry, material);
        robot.position.y = 0.15;
        return robot;
    }

    //Draw pointlight.
    drawLight(x, y, z, intensity, distance, setColor, light) {
        var light = new THREE.PointLight(setColor, intensity, distance);
        this.drawLensflare(light)
        light.position.set(x, y, z);
        return light;
    }

    //Draw lensflare.
    drawLensflare(light) {
        if (light.distance == 0) {
            var textureLoader = new THREE.TextureLoader();
            var textureFlare = textureLoader.load("Textures/lensflare.png");
            var lensflare = new THREE.Lensflare();
            lensflare.addElement(new THREE.LensflareElement(textureFlare, 512, 0));
            light.add(lensflare);
        }
    }

    //Draw box.
    drawWall(witdh, height, depth, x, y, z, giveColor) {
        var geometry = new THREE.BoxGeometry(witdh, height, depth);
        var material = new THREE.MeshLambertMaterial({ color: giveColor });
        var storage = new THREE.Mesh(geometry, material);
        storage.position.set(x, y, z);
        return storage;
    }

    //Draw object.
    drawOBJModel(modelPath, modelName, texturePath, textureName, onload,) {
        new THREE.MTLLoader()
            .setPath(texturePath)
            .load(textureName, function (materials) {
                materials.preload();
                new THREE.OBJLoader()
                    .setMaterials(materials)
                    .setPath(modelPath)
                    .load(modelName, function (object) {
                        onload(object);
                    });

            });
    }

    //Draw cone.
    drawCone(x, y, z, radius, setColor) {
        var geometry = new THREE.ConeGeometry(radius, y, 32);
        var material = new THREE.MeshBasicMaterial({ color: setColor, transparent: true, opacity: 0.2 });
        cone = new THREE.Mesh(geometry, material);
        cone.position.x = x;
        cone.position.y = y / 2;
        cone.position.z = z;
        return cone; 
    }

    //Draw node or synape.
    drawNodeOrSyn(x, z, width, depth, setColor, setBool) {
        var material = new THREE.LineBasicMaterial({ color: setColor });
        var geometry = new THREE.Geometry();
        if (setBool == false) {
            //draw node
            geometry.vertices.push(
                new THREE.Vector3(x, 10, z),
                new THREE.Vector3(x, -10, z)
            );
        }
        else if (setBool == true) {
            //draw synape
            geometry.vertices.push(
                new THREE.Vector3(x, 10, z),
                new THREE.Vector3(width, 15, depth),
                new THREE.Vector3(width, 10, depth)
            );
        }
        return new THREE.Line(geometry, material);
    }
}