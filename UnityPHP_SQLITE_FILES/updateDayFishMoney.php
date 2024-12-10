<?php
// Set the SQLite database file
$dbFile = "tese.db";
$conn = new PDO("sqlite:" . $dbFile);
$conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

if (!$conn) {
    echo "Connection could not be established.<br />";
    die("Error: Unable to connect to SQLite database.");
}

if (isset($_POST['PlayerId']) && isset($_POST['Days']) && isset($_POST['PeixesApanhados']) && isset($_POST['Moedas']) && isset($_POST['Creditos'])) {
    $PlayerId = intval($_POST['PlayerId']);    
    $days = intval($_POST['Days']);
    $fishCaught = intval($_POST['PeixesApanhados']);
    $money = intval($_POST['Moedas']);
    $credits = intval($_POST['Creditos']);

    $sql = "UPDATE MisteriosAquaticos 
            SET Days = :days, 
                PeixesApanhados = :fishCaught, 
                Moedas = :money, 
                Creditos = :credits  
            WHERE PlayerId = :PlayerId";

    $stmt = $conn->prepare($sql);

    $stmt->bindParam(':days', $days, PDO::PARAM_INT);
    $stmt->bindParam(':fishCaught', $fishCaught, PDO::PARAM_INT);
    $stmt->bindParam(':money', $money, PDO::PARAM_INT);
    $stmt->bindParam(':credits', $credits, PDO::PARAM_INT);
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