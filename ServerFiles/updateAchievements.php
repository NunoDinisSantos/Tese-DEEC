<?php
// Set the SQLite database file
$dbFile = "tese.db";
$conn = new PDO("sqlite:" . $dbFile);
$conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

if (!$conn) {
    echo "Connection could not be established.<br />";
    die("Error: Unable to connect to SQLite database.");
}

if (isset($_POST['PlayerId']) && isset($_POST['Tesouro']) && isset($_POST['CoralAnciao']) && isset($_POST['PesquisaPerdida']) && isset($_POST['JoiaTemplo']) && isset($_POST['JoiaBarco']) && isset($_POST['GeloAntigo'])) {
    $PlayerId = intval($_POST['PlayerId']);    
    $tesouro = intval($_POST['Tesouro']);
    $coralAnciao = intval($_POST['CoralAnciao']);
    $pesquisaPerdida = intval($_POST['PesquisaPerdida']);
    $joiaTemplo = intval($_POST['JoiaTemplo']);
    $joiaBarco = intval($_POST['JoiaBarco']);
    $geloAntigo = intval($_POST['GeloAntigo']);

    $sql = "UPDATE Achievements 
            SET tesouro = :tesouro, 
                coralAnciao = :coralAnciao, 
                pesquisaPerdida = :pesquisaPerdida, 
                joiaTemplo = :joiaTemplo, 
                joiaBarco = :joiaBarco, 
                geloAntigo = :geloAntigo 
            WHERE PlayerId = :PlayerId";

    $stmt = $conn->prepare($sql);

    $stmt->bindParam(':tesouro', $tesouro, PDO::PARAM_INT);
    $stmt->bindParam(':coralAnciao', $coralAnciao, PDO::PARAM_INT);
    $stmt->bindParam(':pesquisaPerdida', $pesquisaPerdida, PDO::PARAM_INT);
    $stmt->bindParam(':joiaTemplo', $joiaTemplo, PDO::PARAM_INT);
    $stmt->bindParam(':joiaBarco', $joiaBarco, PDO::PARAM_INT);
    $stmt->bindParam(':geloAntigo', $geloAntigo, PDO::PARAM_INT);
    $stmt->bindParam(':PlayerId', $PlayerId, PDO::PARAM_INT);

    try {
        $stmt->execute();
        echo json_encode(["status" => "success", "message" => "Achievements updated successfully"]);
    } catch (PDOException $e) {
        echo json_encode(["status" => "error", "message" => $e->getMessage()]);
    }

    // Close the connection
    $conn = null;
} else {
    echo json_encode(["status" => "error", "message" => "Invalid data"]);
}
?>