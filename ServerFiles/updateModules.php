<?php
// Set the SQLite database file
$dbFile = "tese.db";
$conn = new PDO("sqlite:" . $dbFile);
$conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

if (!$conn) {
    echo "Connection could not be established.<br />";
    die("Error: Unable to connect to SQLite database.");
}

if (isset($_POST['PlayerId'])) {
    $PlayerId = intval($_POST['PlayerId']);    
    $deepModule = intval($_POST['ModuloProfundidade']);
    $tempModule = intval($_POST['ModuloTemperatura']);
    $ReelModule = intval($_POST['ModuloReel']);
    $storageModule = intval($_POST['ModuloStorage']);
    $flashlight = intval($_POST['Lanterna']);

    $sql = "UPDATE MisteriosAquaticos 
            SET ModuloProfundidade = :deepModule, 
                ModuloTemperatura = :tempModule, 
                ModuloReel = :ReelModule, 
                ModuloStorage = :storageModule, 
                Lanterna = :flashlight 
            WHERE PlayerId = :PlayerId";

    $stmt = $conn->prepare($sql);

    $stmt->bindParam(':deepModule', $deepModule, PDO::PARAM_INT);
    $stmt->bindParam(':tempModule', $tempModule, PDO::PARAM_INT);
    $stmt->bindParam(':ReelModule', $ReelModule, PDO::PARAM_INT);
    $stmt->bindParam(':storageModule', $storageModule, PDO::PARAM_INT);
    $stmt->bindParam(':flashlight', $flashlight, PDO::PARAM_INT);
    $stmt->bindParam(':PlayerId', $PlayerId, PDO::PARAM_INT);

    try {
        $stmt->execute();
        echo json_encode(["status" => "success", "message" => "Data updated successfully"]);
    } catch (PDOException $e) {
        echo json_encode(["status" => "error", "message" => $e->getMessage()]);
    }

    // Close the connection
    $conn = null;
} else {
    echo json_encode(["status" => "error", "message" => "Invalid data"]);
}
?>